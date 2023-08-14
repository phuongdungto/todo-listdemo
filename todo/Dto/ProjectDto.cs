using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace todo.Dto
{
    public class ProjectDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public UserDto? User { get; set; }
        public ICollection<TasksDto>? Tasks { get; set; }
        public ICollection<ProjectDetailDto>? ProjectDetails { get; set; }
    }

    public class CreateProjectDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public ICollection<ProjectDetailDto>? ProjectDetails { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public Guid? UserId { get; set; }
        public ICollection<ProjectDetailDto>? ProjectDetails { get; set; }
    }
    public class ProjectDetailDto
    {
        [Required]
        public Guid? UserId { get; set; }
        public UserDto? User { get; set; }
        public Guid? ProjectId { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ProjectDto? Project { get; set; }
    }

    public class GetProjectsResponse
    {
        public int totalPages { get; set; }
        public ICollection<ProjectDto> Projects { get; set; }
    }
}
