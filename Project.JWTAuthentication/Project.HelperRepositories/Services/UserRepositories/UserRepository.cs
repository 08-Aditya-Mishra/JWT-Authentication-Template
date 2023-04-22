using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.HelperRepositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperRepositories.Services.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _Context;
        public UserRepository(ApplicationDbContext Context)
        {
            _Context = Context;
        }
        public Task<ApplicationUser> GetByEmail(string email)
        {
            return Task.FromResult(_Context.Users.FirstOrDefault(u => u.Email == email));

        }

        public Task<ApplicationUser> GetByUsername(string username)
        {
            return Task.FromResult(_Context.Users.FirstOrDefault(u => u.UserName == username));
        }
    }
}
