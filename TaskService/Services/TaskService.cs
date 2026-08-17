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
            _logger.LogInformation("Task: @{task}",task);
            await _unitofwork.Tasks.DeletedAsync(task);
            await _unitofwork.SaveChange();
            return new ApiResponse(task);

        }
        public async  Task<ApiResponse> AddTaskAsync(TaskModel tasks)
        {
            var httpContext = _contextAccessor.HttpContext;

            var userId = httpContext?.User?.FindFirst("sub")?.Value 
                      ?? httpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                      ?? httpContext?.Request.Headers["X-User-Id"].ToString();

            _logger.LogInformation($"[TaskService AddTaskAsync] User ID: {userId}");

            if (string.IsNullOrEmpty(tasks.Title))
            {
                _logger.LogWarning("Title is empty");
                return new ApiResponse(400, "Fields Cannot be empty");
            }

            // Set the UserId on the task
            tasks.AuthorId = userId;

            await _unitofwork.Tasks.AddAsync(tasks);
            await _unitofwork.SaveChange();
            return new ApiResponse(tasks,"Successfully Added to Database");

        }

        public async Task<ApiResponse> GetTaskAsync()
        {
            var httpContext = _contextAccessor.HttpContext;

            // Try to get user ID from JWT claims (populated by authentication middleware)
            var userId = httpContext?.User?.FindFirst("sub")?.Value 
                      ?? httpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                      ?? httpContext?.Request.Headers["X-User-Id"].ToString(); // Fallback to header if present

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("[TaskService GetTaskAsync] SECURITY WARNING: User ID could not be retrieved from claims or headers!");
                _logger.LogWarning($"[TaskService GetTaskAsync] User Identity: {httpContext?.User?.Identity?.IsAuthenticated}");
                _logger.LogWarning($"[TaskService GetTaskAsync] Claims count: {httpContext?.User?.Claims.Count()}");
                return new ApiResponse(401, "User Identity could not be verified by the service.");
            }

            _logger.LogInformation($"[TaskService GetTaskAsync] Retrieved user ID: {userId}");
            var tasks = await _unitofwork.Tasks.GetAllAsync(userId);
            _logger.LogDebug(tasks.ToString());

            if (!tasks.Any()) 
                return new ApiResponse(404, "No Tasks found");

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
