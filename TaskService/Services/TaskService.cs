using AuthService.Helpers;
using TaskService.Model;
using TaskService.Repository;

namespace TaskService.Services
{
    public class TaskServices : ITaskService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<TaskServices> _logger;

        public TaskServices(IUnitofWork unitofWork, IHttpContextAccessor contextAccessor,ILogger<TaskServices> logger)
        {
            _unitofwork = unitofWork;
            _contextAccessor = contextAccessor;
            _logger = logger;
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
        public async  Task<ApiResponse> AddTaskAsync(TaskModel tasks)
        {
            var header = _contextAccessor.HttpContext?.Request.Headers;
            header.TryGetValue("X-User-Id", out var userid);
            _logger.LogInformation(userid);
            if (string.IsNullOrEmpty(tasks.Title))
            {
                _logger.LogWarning("Title is empty");
                return new ApiResponse(400, "Fields Cannot be empty");
            }
            await _unitofwork.Tasks.AddAsync(tasks);
            await _unitofwork.SaveChange();
            return new ApiResponse(tasks,"Successfully Added to Database");

        }

        public async Task<ApiResponse> GetTaskAsync()
        {
            var headers = _contextAccessor.HttpContext?.Request.Headers;

            // Check if header is missing or empty string
            if (headers == null || !headers.TryGetValue("X-User-Id", out var userid) || string.IsNullOrWhiteSpace(userid))
            {
                _logger.LogError("SECURITY WARNING: X-User-Id header was missing or empty!");
                return new ApiResponse(401, "User Identity could not be verified by the service.");
            }
            _logger.LogInformation(userid);
            var tasks = await _unitofwork.Tasks.GetAllAsync(userid);
            _logger.LogDebug(tasks.ToString());
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
