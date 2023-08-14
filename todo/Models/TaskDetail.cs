using System.ComponentModel.DataAnnotations.Schema;

namespace todo.Models
{
    public class TaskDetail
    {
        [Column("task_id")]
        public Guid TasksId { get; set; }
        public Tasks Tasks { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
