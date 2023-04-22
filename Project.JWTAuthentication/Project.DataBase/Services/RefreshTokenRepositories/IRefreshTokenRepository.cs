using Project.HelperRepositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperRepositories.Services.RefreshTokenRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByToken(string token);

        Task Create(RefreshToken refreshToken);

        Task Delete(Guid id);

        Task DeleteAll(Guid userId);
    }
}
