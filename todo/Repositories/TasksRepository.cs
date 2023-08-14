using todo.Dto;
using todo.Models;

namespace todo.Repositories
{
    public interface ITasksRepository
    {
        Task<Tasks> CreateTask(CreateTaskDto Tasks);
        Task<Tasks> UpdateTask(UpdateTaskDto Tasks, Guid id);
        Task DeleteTask(Guid id);
        Task<Tasks> GetTask(Guid id);
        Task<IQueryable<Tasks>> GetTasks();
    }
}
