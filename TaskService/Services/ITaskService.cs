using AuthService.Helpers;
using TaskService.Model;

namespace TaskService.Services
{
    public interface ITaskService
    {
        Task <ApiResponse> GetTaskAsync();
        Task<ApiResponse> GetTaskById(string id);
        Task<ApiResponse> DeleteTaskAsync(string id);
        Task<ApiResponse> UpdateTaskAsync(TaskModel entity, string id);
    }
}
