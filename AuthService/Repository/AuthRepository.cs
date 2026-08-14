using AuthService.Data;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task<User?> GetByEmail(string Email)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.Email == Email);
        }
    }
}
