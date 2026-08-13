namespace TaskService.Model
{
    public class TaskModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; } = false;
        public string AuthorId { get; set; } = string.Empty;

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime DeadLine { get; set; }
    }
}
