using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Context;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Models;
using System.Text;

using System.Security.Cryptography;
namespace PerformanceSurvey.Repository
{
    public class AuthLoginRepository : IAuthLoginRepository
    {
        private readonly ApplicationDbContext _context;
       
        public AuthLoginRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task CreateAdminUserAsync(User adminUser)
        {
            _context.users.Add(adminUser);
            await _context.SaveChangesAsync();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserEmail == email && !u.IsDisabled);
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            return await _context.users
                .FirstOrDefaultAsync(u => u.UserEmail == email && u.Password == hashedPassword);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }



        private readonly List<string> _revokedTokens = new List<string>();
        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            var revokedToken = await _context.Tokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.AuthToken == token && t.RevokedOn != null);

            return revokedToken != null;
        }

        public async Task RevokeTokenAsync(string token)
        {
            var tokenEntity = await _context.Tokens
                .FirstOrDefaultAsync(t => t.AuthToken == token);

            if (tokenEntity != null)
            {
                tokenEntity.RevokedOn = DateTime.UtcNow;
                _context.Tokens.Update(tokenEntity);
                await _context.SaveChangesAsync();
            }
        }

    }
}
