using TaskService.Data;
using Microsoft.EntityFrameworkCore;
using TaskService.Model;

namespace TaskService.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        public TaskRepository(AppDbContext context)
        {
            _context = context;

        }
        public async Task DeletedAsync(TaskModel tasks)
        {
            _context.Remove(tasks);
        }

        public async Task<List<TaskModel>> GetAllAsync(string id)
        {
            var res =  _context.tasks.ToList();
            return res;
        }

        public async Task<TaskModel?> GetByIdAsync(string id)
        {
            var res = await _context.tasks.FindAsync(id);
            return res;
        }

        public async Task UpdateAsync(TaskModel entity)
        {
            _context.Update(entity);
        }
    }
}
