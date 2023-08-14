using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Sieve.Models;
using Sieve.Services;

using todo.Dto;
using todo.Repositories;

namespace todo.Controllers
{
    [Route("tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITasksRepository tasksRepository;
        private readonly IMapper mapper;
        private readonly ISieveProcessor sieveProcessor;
        public TaskController(ITasksRepository tasksRepository, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            this.tasksRepository = tasksRepository;
            this.mapper = mapper;
            this.sieveProcessor = sieveProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto input)
        {
            var tasks = await tasksRepository.CreateTask(input);
            var tasksDto = mapper.Map<TasksDto>(tasks);
            return Ok(tasksDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(UpdateTaskDto input, Guid id)
        {
            var tasks = await tasksRepository.UpdateTask(input, id);
            var tasksDto = mapper.Map<TasksDto>(tasks);
            return Ok(tasksDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await tasksRepository.DeleteTask(id);
            return Ok("Deleted task success");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var tasks = await tasksRepository.GetTask(id);
            var tasksDto = mapper.Map<TasksDto>(tasks);
            return Ok(tasksDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] SieveModel model)
        {
            var tasks = await tasksRepository.GetTasks();
            var pageSize = model.PageSize;
            var page = model.Page;
            model.PageSize = null;
            model.Page = null;
            var totalRecord = sieveProcessor.Apply(model, tasks).Count();
            model.PageSize = pageSize;
            model.Page = page;
            var TaskEntity = sieveProcessor.Apply(model, tasks);
            var TaskDto = mapper.Map<List<TasksDto>>(TaskEntity);
            if (model.PageSize == null)
            {
                model.PageSize = totalRecord;
            }
            return Ok(new GetTasksResponse
            {
                totalPages = (int)Math.Ceiling((double)totalRecord / (double)model.PageSize),
                Tasks = TaskDto.ToList()
            });
        }
    }
}
