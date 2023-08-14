using Sieve.Attributes;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo.Models
{
    [Table("projects")]
    public class Project : Auditable
    {
        [Key]
        [Column("id")]
        [Sieve(CanFilter = true)]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Column("user_id")]
        [Sieve(CanFilter = true)]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
        public ICollection<ProjectDetail> ProjectDetails { get; set; }
    }
}
