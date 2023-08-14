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
    public class ProjectService : IProjectRepository
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;
        public ProjectService(TodoDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }
        async Task<Project> IProjectRepository.CreateProject(CreateProjectDto input)
        {
            var user = await context.Users.SingleOrDefaultAsync(p => p.Id == input.UserId);
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }
            var details = input.ProjectDetails;
            input.ProjectDetails = null;

            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                var project = mapper.Map<Project>(input);
                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();

                if (details != null)
                {
                    foreach (var item in details)
                    {
                        item.ProjectId = project.Id;
                    }
                    var projectDetails = mapper.Map<ICollection<ProjectDetail>>(details);
                    await context.ProjectDetails.AddRangeAsync(projectDetails);
                    await context.SaveChangesAsync();
                }
                await myTransaction.CommitAsync();
                return project;
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        async Task IProjectRepository.DeleteProject(Guid id)
        {
            var project = await context.Projects
                .Include(x => x.Tasks)
                .Include(x => x.ProjectDetails)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                throw new BadRequestException("Project not found");
            }
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.Tasks.RemoveRange(project.Tasks);
                context.ProjectDetails.RemoveRange(project.ProjectDetails);
                context.Remove<Project>(project);
                await context.SaveChangesAsync();
                await myTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        async Task<Project> IProjectRepository.GetProject(Guid id)
        {
            var query = context.Projects.Where(p => p.Id == id)
            .Include(x => x.User)
            .Include(x => x.ProjectDetails)
            .ThenInclude(x => x.User)
            .Include(x => x.Tasks);
            var project = await query.FirstOrDefaultAsync();

            if (project == null)
            {
                throw new BadRequestException("Project not found");
            };
            return project;
        }

        async Task<IQueryable<Project>> IProjectRepository.GetProjects()
        {
            var project = from p in context.Projects
                          join u in context.Users on p.UserId equals u.Id
                          select new Project
                          {
                              DateCreated = p.DateCreated,
                              DateUpdated = p.DateUpdated,
                              Id = p.Id,
                              Name = p.Name,
                              UserId = p.UserId,
                              User = u
                          };
            return project;
        }

        async Task<Project> IProjectRepository.UpdateProject(UpdateProjectDto input, Guid id)
        {
            var project = await context.Projects.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (project == null)
            {
                throw new BadRequestException("Project not found");
            }
            var details = input.ProjectDetails;
            input.ProjectDetails = null;
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                MapObjects.Assign<Project, UpdateProjectDto>(project, input);
                context.Projects.Update(project);
                await context.SaveChangesAsync();
                if (details != null)
                {
                    var projectDetails = await context.ProjectDetails.Where(pd => pd.ProjectId == id).ToListAsync();
                    context.ProjectDetails.RemoveRange(projectDetails);
                    await context.SaveChangesAsync();
                    foreach (var item in details)
                    {
                        item.ProjectId = project.Id;
                    }
                    var newProjectDetails = mapper.Map<ICollection<ProjectDetail>>(details);
                    await context.ProjectDetails.AddRangeAsync(newProjectDetails);
                    await context.SaveChangesAsync();
                }
                await myTransaction.CommitAsync();
                return project;
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
