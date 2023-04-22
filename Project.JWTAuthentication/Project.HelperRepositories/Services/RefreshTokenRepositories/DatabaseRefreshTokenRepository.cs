using Microsoft.EntityFrameworkCore;
using Project.HelperRepositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperRepositories.Services.RefreshTokenRepositories
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseRefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(RefreshToken refreshToken)
        {
            _context.refreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            RefreshToken refreshToken = await _context.refreshTokens.FindAsync(id);
            if (refreshToken != null)
            {
                _context.refreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(Guid userId)
        {
            IEnumerable<RefreshToken> refreshTokens = await _context.refreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.refreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _context.refreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
