using todo.Dto;
using todo.Models;

namespace todo.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(CreateUsersDto user);
        Task<User> UpdateUser(Guid id, UpdateUsersDto user);
        Task DeleteUser(Guid id);
        Task<User> GetUser(Guid id);
        Task<IQueryable<User>> GetUsers();
        Task<AuthResponse> Login(AuthDto user);
    }
}
