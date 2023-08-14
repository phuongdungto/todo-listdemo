using Sieve.Attributes;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using todo.Enums;
using todo.Models;

namespace todo.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Fullname { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }

        [Sieve(CanFilter = true)]
        public string Role { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Project>? Projects { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<ProjectDetail>? ProjectDetails { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<TaskDetail>? TasksDetails { get; set; }
    }

    public class CreateUsersDto
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [EnumDataType(typeof(Roles))]
        public Roles Role { get; set; }
    }

    public class UpdateUsersDto
    {
        public string? Fullname { get; set; }
        [MinLength(8)]
        public string? Password { get; set; }
    }
    public class AuthDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string AccessToken { get; set; }
        public AuthResponse(Guid id, string email, string fullname, string accessToken)
        {
            Id = id;
            Email = email;
            Fullname = fullname;
            AccessToken = accessToken;
        }
    }

    public class GetUsersResponse
    {
        public int totalPages { get; set; }
        public ICollection<UserDto> Users { get; set; }

    }
}
