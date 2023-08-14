using System.ComponentModel.DataAnnotations.Schema;

namespace todo.Models
{
    [Table("project_details")]
    public class ProjectDetail
    {
        public Project Project { get; set; }
        [Column("project_id")]
        public Guid ProjectId { get; set; }
        public User User { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
    }
}
