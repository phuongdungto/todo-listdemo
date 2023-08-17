using Microsoft.AspNetCore.Identity;

using Sieve.Attributes;

using System.ComponentModel.DataAnnotations.Schema;

namespace todo.Models
{
    [Table("users")]
    public class User : IdentityUser
    {
        /*[Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("fullname")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        [Column("email")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }

        [Column("role")]
        [EnumDataType(typeof(Roles))]
        [DefaultValue(Roles.user)]
        [Sieve(CanFilter = true)]
        public Roles Role { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }*/
        [Column("fullname")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string Fullname { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<ProjectDetail> ProjectDetails { get; set; }
        public ICollection<TaskDetail>? TasksDetails { get; set; }
    }
}
