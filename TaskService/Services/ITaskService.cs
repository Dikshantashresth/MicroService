using AuthService.Helpers;
using TaskService.Model;

namespace TaskService.Services
{
    public interface ITaskService
    {
        Task<ApiResponse> GetTaskAsync();
        Task<ApiResponse> GetTaskById(string id);
        Task<ApiResponse> AddTaskAsync(TaskModel tasks);
        Task<ApiResponse> DeleteTaskAsync(string id);
        Task<ApiResponse> UpdateTaskAsync(TaskModel entity, string id);
    }
}
