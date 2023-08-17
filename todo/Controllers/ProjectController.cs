using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sieve.Models;
using Sieve.Services;

using todo.Dto;
using todo.Repositories;

namespace todo.Controllers
{
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly IMapper mapper;
        private readonly ISieveProcessor sieveProcessor;
        public ProjectController(IProjectRepository _projectRepository, IMapper _mapper, ISieveProcessor _sieveProcessor)
        {
            projectRepository = _projectRepository;
            mapper = _mapper;
            sieveProcessor = _sieveProcessor;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
        public async Task<IActionResult> CreateProject(CreateProjectDto project)
        {
            var newProject = await projectRepository.CreateProject(project);
            var projectDto = mapper.Map<ProjectDto>(newProject);
            return Ok(projectDto);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
        public async Task<IActionResult> DeteleProject(Guid id)
        {
            await projectRepository.DeleteProject(id);
            return Ok("Deleted project success");
        }
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
        public async Task<IActionResult> UpdateProject(UpdateProjectDto project, Guid id)
        {
            var newProject = await projectRepository.UpdateProject(project, id);
            var projectDto = mapper.Map<ProjectDto>(newProject);
            return Ok(projectDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var projects = await projectRepository.GetProject(id);
            var projectsDto = mapper.Map<ProjectDto>(projects);
            return Ok(projectsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] SieveModel model)
        {
            var projects = await projectRepository.GetProjects();
            var pageSize = model.PageSize;
            var page = model.Page;
            model.PageSize = null;
            model.Page = null;
            var totalRecord = sieveProcessor.Apply(model, projects).Count();
            model.PageSize = pageSize;
            model.Page = page;
            var projectsEntity = sieveProcessor.Apply(model, projects);
            var projectsDto = mapper.Map<List<ProjectDto>>(projectsEntity);
            if (model.PageSize == null)
            {
                model.PageSize = totalRecord;
            }
            return Ok(new GetProjectsResponse
            {
                totalPages = (int)Math.Ceiling((double)totalRecord / (double)model.PageSize),
                Projects = projectsDto.ToList()
            });
        }
    }
}
