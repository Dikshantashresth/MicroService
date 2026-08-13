using TaskService.Model;

namespace TaskService.Repository
{
    public interface ITaskRepository
    {
        Task<List<TaskModel>> GetAllAsync(string id);
        Task<TaskModel?> GetByIdAsync(string id);
        Task DeletedAsync(TaskModel entity);
        Task UpdateAsync(TaskModel entity);
    }
}
