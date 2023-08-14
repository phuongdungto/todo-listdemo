using AutoMapper;

using todo.Dto;
using todo.Models;

namespace todo.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateUsersDto, User>();
            CreateMap<UpdateUsersDto, User>();

            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<ProjectDetailDto, ProjectDetail>();

            CreateMap<CreateTaskDto, Tasks>();
            CreateMap<UpdateTaskDto, Tasks>();
            CreateMap<TaskDetailDto, TaskDetail>();

            CreateMap<Project, ProjectDto>();
            CreateMap<Tasks, TasksDto>();
            CreateMap<User, UserDto>();
            CreateMap<TaskDetail, TaskDetailDto>();
            CreateMap<ProjectDetail, ProjectDetailDto>();

        }
    }
}
