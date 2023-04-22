using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.HelperRepositories.Models;
using Project.HelperRepositories.Models.Dto;
using Project.HelperRepositories.Services.Authenticators;
using Project.HelperRepositories.Services.RefreshTokenRepositories;
using Project.HelperRepositories.Services.Responses;
using Project.HelperRepositories.Services.TokenGenerators;
using Project.HelperRepositories.Services.TokenValidators;
using Project.HelperRepositories.Services.UserRepositories;
using Project.JWTAuthentication.Helpers;
using System.Security.Claims;

namespace Project.JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly Authenticator _authenticator;
        UserManager<ApplicationUser> _userManager;
        public AuthenticationController(Authenticator authenticator, IUserRepository userRepository, UserManager<ApplicationUser> userManager, RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _authenticator = authenticator;
            _userRepository = userRepository;
            _userManager = userManager;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;

        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterDto value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            ApplicationUser existingUserName =  await _userRepository.GetByUsername(value.UserName);
            if ((existingUserName!=null))
            {
                return Conflict(new ErrorHandler("Username already exists."));
            }

            ApplicationUser existingEmail = await _userRepository.GetByEmail(value.Email);
            if (existingEmail != null)
            {
                return Conflict(new ErrorHandler("Email already exists."));
            }

            var user = new ApplicationUser
            {
                UserName = value.UserName,
                Email = value.Email,
                Name = value.Name
            };
            await _userManager.CreateAsync(user,value.Password);
            return Ok();
        }

        [HttpPost]
        [Route("LoginUser")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            ApplicationUser usernameExists = await _userRepository.GetByUsername(loginRequest.UserName);
            if (usernameExists==null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }

            AuthenticatedUserResponse response = await _authenticator.Authenticate(user);

            return Ok(response);
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorHandler("Invalid refresh token."));
            }

            RefreshToken refreshTokenDTO = await _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return NotFound(new ErrorHandler("Invalid refresh token."));
            }

            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            ApplicationUser user = await _userManager.FindByIdAsync(refreshTokenDTO.UserId.ToString());
            if (user == null)
            {
                return NotFound(new ErrorHandler("User not found."));
            }

            AuthenticatedUserResponse response = await _authenticator.Authenticate(user);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");

            if (!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            await _refreshTokenRepository.DeleteAll(userId);

            return NoContent();
        }

        private IActionResult BadRequestModelState()
        {
            IEnumerable<string> errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new ErrorHandler(errorMessages));
        }
    }
}
