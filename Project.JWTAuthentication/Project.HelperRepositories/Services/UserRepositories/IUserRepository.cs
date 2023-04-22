using Project.HelperRepositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperRepositories.Services.UserRepositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByEmail(string email);
        Task<ApplicationUser> GetByUsername(string username);
    }
}
