using Microsoft.AspNetCore.Mvc;

namespace Project.JWTAuthentication.Helpers
{
    public class ErrorHandler
    {
        public IEnumerable<string> ErrorMessages { get; set; }
        public ErrorHandler(string errorMessage) : this(new List<string>() { errorMessage }) { }
        public ErrorHandler(IEnumerable<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }
    }
}
