using Sieve.Attributes;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using todo.Enums;

namespace todo.Models
{
    [Table("tasks")]
    public class Tasks : Auditable
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Column("start_date")]
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? EndDate { get; set; }

        [Column("status")]
        [EnumDataType(typeof(TasksStatus))]
        [DefaultValue(TasksStatus.inProcess)]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Status { get; set; }

        public ICollection<TaskDetail> TasksDetails { get; set; }

        [Column("project_id")]
        [Sieve(CanFilter = true)]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
