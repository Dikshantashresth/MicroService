using AuthService.Helpers;
using TaskService.Repository;
using TaskService.Model;

namespace TaskService.Services
{
    public class TaskServices : ITaskService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IHttpContextAccessor _contextAccessor;

        public TaskServices(IUnitofWork unitofWork, IHttpContextAccessor contextAccessor)
        {
            _unitofwork = unitofWork;
            _contextAccessor = contextAccessor;

        }
        public async Task<ApiResponse> DeleteTaskAsync(string id)
        {
            if (id == null) return new ApiResponse(400, "Id is invalid");
            var task = await _unitofwork.Tasks.GetByIdAsync(id);
            if (task == null) return new ApiResponse(400, "Task doesnot exist");
            await _unitofwork.Tasks.DeletedAsync(task);
            await _unitofwork.SaveChange();
            return new ApiResponse(task);

        }

        public async Task<ApiResponse> GetTaskAsync()
        {
            var headers =  _contextAccessor.HttpContext?.Request.Headers;
            headers.TryGetValue("X-User-Id", out var userid);
            var tasks = await _unitofwork.Tasks.GetAllAsync(userid);
            if (!tasks.Any()) return new ApiResponse(404, "No Tasks found");
            return new ApiResponse(tasks);
        }

        public async Task<ApiResponse> GetTaskById(string id)
        {
            if (id == null) return new ApiResponse(400, "Id is invalid");
            var task = await _unitofwork.Tasks.GetByIdAsync(id);
            if (task == null) return new ApiResponse(400, "Task doesnot exist");
            return new ApiResponse(task);
        }

        public async Task<ApiResponse> UpdateTaskAsync(TaskModel entity, string id)
        {
            if (id == null) return new ApiResponse(400, "Id is invalid");
            var task = await _unitofwork.Tasks.GetByIdAsync(id);
            if (task == null) return new ApiResponse(400, "Task doesnot exist");
            task.Title = entity.Title;
            task.isCompleted = entity.isCompleted;
            task.DeadLine = entity.DeadLine;
            await _unitofwork.Tasks.UpdateAsync(task);
            await _unitofwork.SaveChange();
            return new ApiResponse(task);
        }
    }
}
