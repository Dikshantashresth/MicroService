using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Model;
using TaskService.Repository;
using TaskService.Services;

namespace TaskService.Controllers
{
    /// <response code="401">Unauthorized</response>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController:ControllerBase
    {
        private readonly ITaskService _taskService;
        
        public TasksController(ITaskService taskservice)
        {
            _taskService = taskservice;

        }
        /// <summary>
        /// Add new task to the database.
        /// </summary>
        /// <param name="Task">Details about the task from the user that is needed to be stored.</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> PostTask([FromBody] TaskModel Task)
        {
            var newTask = await _taskService.AddTaskAsync(Task);
            return StatusCode(newTask.Status, newTask);
        }
        /// <summary>
        /// Fetches all the tasks of the user using user id from header "X-User-Id"
        /// </summary>
        /// <returns>All the tasks from the user</returns>
        [HttpGet()]
        public async Task<IActionResult> GetTasks()
        {
            var alltasks = await _taskService.GetTaskAsync();
            return StatusCode(alltasks.Status, alltasks);
        }
        /// <summary>
        /// Get user tasks by id. 
        /// </summary>
        /// <param name="id">Api is fetched and id is passed through param</param>
        /// <returns>Task with particular id.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTasksByid(string id)
        {
            var TaskById = await _taskService.GetTaskById(id);
            return StatusCode(TaskById.Status, TaskById);
        }

        /// <summary>
        /// Controller to update the task using id and the newly updated field passed into taskservice update method.
        /// </summary>
        /// <param name="id">Unique id of the task</param>
        /// <param name="tasks">new updated fields</param>
        /// <returns>Returns Updated task</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id,[FromBody] TaskModel tasks)
        {
            
            var updatedTask = await _taskService.UpdateTaskAsync(tasks, id);
            return StatusCode(updatedTask.Status, updatedTask);
        }

        /// <summary>
        /// Finds whether the task exists or if task exists it deletes the task.
        /// </summary>
        /// <param name="id">Unique id to delete the particular task requested.</param>
        /// <returns>Returns the deleted task</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var deletedTask = await _taskService.DeleteTaskAsync(id);
            return StatusCode(deletedTask.Status, deletedTask);
        }
    }
}
