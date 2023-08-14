using AutoMapper;

using Microsoft.EntityFrameworkCore;

using todo.Data;
using todo.Dto;
using todo.Exceptions;
using todo.Helpers;
using todo.Models;
using todo.Repositories;

namespace todo.Services
{
    public class TaskService : ITasksRepository
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;
        public TaskService(TodoDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }
        async Task<Tasks> ITasksRepository.CreateTask(CreateTaskDto input)
        {
            var details = input.TasksDetails;
            input.TasksDetails = null;
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                var users = await context.ProjectDetails
                    .Where(pd => pd.ProjectId == input.ProjectId)
                    .ToListAsync();
                var taskDetails = mapper.Map<List<TaskDetail>>(details);
                bool checkUser = users.Any(user1 => taskDetails.Any(user2 => user2.UserId == user1.UserId));
                if (checkUser == false)
                {
                    throw new BadRequestException("users not participating in the project");
                }
                var tasks = mapper.Map<Tasks>(input);
                await context.Tasks.AddAsync(tasks);
                await context.SaveChangesAsync();
                if (taskDetails != null)
                {
                    foreach (var item in taskDetails)
                    {
                        item.TasksId = tasks.Id;
                    }
                    await context.TasksDetals.AddRangeAsync(taskDetails);
                    await context.SaveChangesAsync();
                }
                await myTransaction.CommitAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        async Task ITasksRepository.DeleteTask(Guid id)
        {
            var tasks = await context.Tasks.Where(t => t.Id == id)
                .Include(x => x.TasksDetails)
                .SingleOrDefaultAsync();
            if (tasks == null)
            {
                throw new BadRequestException("Task not found");
            }
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.TasksDetals.RemoveRange(tasks.TasksDetails);
                await context.SaveChangesAsync();
                context.Tasks.Remove(tasks);
                await context.SaveChangesAsync();
                await myTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        async Task<Tasks> ITasksRepository.GetTask(Guid id)
        {
            var tasks = await context.Tasks.Where(t => t.Id == id)
                .Include(x => x.TasksDetails)
                .ThenInclude(x => x.User)
                .SingleOrDefaultAsync();
            if (tasks == null)
            {
                throw new NotFoundException("Task not found");
            }
            return tasks;
        }

        async Task<IQueryable<Tasks>> ITasksRepository.GetTasks()
        {
            var query = from t in context.Tasks
                        select t;
            return query;
        }

        async Task<Tasks> ITasksRepository.UpdateTask(UpdateTaskDto input, Guid id)
        {
            var tasks = await context.Tasks.Where(t => t.Id == id).SingleOrDefaultAsync();
            if (tasks == null)
            {
                throw new BadRequestException("Task not found");
            }
            var details = input.TasksDetails;
            input.TasksDetails = null;
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                MapObjects.Assign(tasks, input);
                context.Tasks.Update(tasks);
                await context.SaveChangesAsync();
                if (details != null)
                {
                    var users = await context.ProjectDetails
                        .Where(pd => pd.ProjectId == input.ProjectId)
                        .ToListAsync();
                    var taskDetails = mapper.Map<List<TaskDetail>>(details);
                    bool checkUser = users.Any(user1 => taskDetails.Any(user2 => user2.UserId == user1.UserId));
                    if (checkUser == false)
                    {
                        throw new BadRequestException("users not participating in the project");
                    }
                    context.TasksDetals.RemoveRange(taskDetails);
                    foreach (var item in taskDetails)
                    {
                        item.TasksId = tasks.Id;
                    }
                    await context.TasksDetals.AddRangeAsync(taskDetails);
                    await context.SaveChangesAsync();
                }
                await myTransaction.CommitAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
