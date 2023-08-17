using todo.Dto;
using todo.Models;

namespace todo.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CreateUser(CreateUsersDto user);
        Task<UserDto> UpdateUser(string id, UpdateUsersDto user);
        Task DeleteUser(string id);
        Task<UserDto> GetUser(string id);
        Task<IQueryable<User>> GetUsers();
        Task<AuthResponse> Login(AuthDto user);
    }
}
