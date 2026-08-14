

using AuthService.Data;

namespace AuthService.Repository
{
    public class UnitOfWork : IUnitofWork
    {
        private readonly AppDbContext _context;
        public IAuthRepository Users { get; set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new AuthRepository(_context);
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
