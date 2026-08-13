using TaskService.Repository;

namespace TaskService.Repository
{
    public interface IUnitofWork
    {
        ITaskRepository Tasks { get; set; }
         Task<int> SaveChange();
    }
}
