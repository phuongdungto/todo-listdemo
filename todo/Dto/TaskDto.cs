using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using todo.Enums;

namespace todo.Dto
{
    public class TasksDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }

        public ICollection<TaskDetailDto>? TasksDetails { get; set; }
        public Guid ProjectId { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ProjectDto? Project { get; set; }
    }

    public class TaskDetailDto
    {
        public Guid? TasksId { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public TasksDto? Tasks { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }

    }
    public class CreateTaskDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [EnumDataType(typeof(TasksStatus))]
        [DefaultValue(TasksStatus.inProcess)]
        public string? Status { get; set; }
        [Required]
        public ICollection<TaskDetailDto> TasksDetails { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [EnumDataType(typeof(TasksStatus))]
        public string? Status { get; set; }
        public ICollection<TaskDetailDto>? TasksDetails { get; set; }
        public Guid? ProjectId { get; set; }
    }

    public class GetTasksResponse
    {
        public int totalPages { get; set; }
        public ICollection<TasksDto> Tasks { get; set; }
    }
}
