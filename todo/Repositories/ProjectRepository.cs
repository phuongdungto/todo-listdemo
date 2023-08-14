using todo.Dto;
using todo.Models;

namespace todo.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateProject(CreateProjectDto project);
        Task<Project> UpdateProject(UpdateProjectDto project, Guid id);
        Task DeleteProject(Guid id);
        Task<Project> GetProject(Guid id);
        Task<IQueryable<Project>> GetProjects();
    }
}
