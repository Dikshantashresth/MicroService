using TaskService.Data;

namespace TaskService.Repository
{
    public class UnitOfWork : IUnitofWork
    {
        public ITaskRepository Tasks { get; set; }
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Tasks = new TaskRepository(_context);
        }

        public async Task<int> SaveChange()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
