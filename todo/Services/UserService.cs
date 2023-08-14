using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using todo.Data;
using todo.Dto;
using todo.Exceptions;
using todo.Helpers;
using todo.Models;
using todo.Repositories;

namespace todo.Services
{
    public class UserService : IUserRepository
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;
        private readonly AppSettings appSettings;
        private const int salt = 10;
        public UserService(TodoDbContext context, IMapper _mapper, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            this.context = context;
            this.mapper = _mapper;
            this.appSettings = optionsMonitor.CurrentValue;
        }

        async Task<User> IUserRepository.CreateUser(CreateUsersDto user)
        {
            var exitsUser = from u in context.Users
                            where u.Email == user.Email
                            select u;
            var exists = await exitsUser.SingleOrDefaultAsync();
            if (exists != null)
            {
                throw new BadRequestException("User already existed");

            }
            var userEntity = mapper.Map<User>(user);
            userEntity.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, salt);
            await context.Users.AddAsync(userEntity);
            await context.SaveChangesAsync();
            return userEntity;
        }

        async Task IUserRepository.DeleteUser(Guid id)
        {
            var exists = from u in context.Users
                         where u.Id == id
                         select u;
            var user = await exists.SingleOrDefaultAsync();
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        async Task<User> IUserRepository.GetUser(Guid id)
        {
            var user = await context.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            return user;
        }

        async Task<User> IUserRepository.UpdateUser(Guid id, UpdateUsersDto input)
        {
            var user = await context.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            MapObjects.Assign<User, UpdateUsersDto>(user, input);
            if (input.Password != null)
            {
                user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, salt);
            }
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }
        async Task<IQueryable<User>> IUserRepository.GetUsers()
        {
            var users = from u in context.Users
                        select u;
            return users;
        }
        async Task<AuthResponse> IUserRepository.Login(AuthDto user)
        {
            var userLogin = await context.Users
                .SingleOrDefaultAsync(p => p.Email == user.Email);
            if (userLogin == null)
            {
                throw new BadRequestException("invalid username/password");
            }
            var hasPassword = BCrypt.Net.BCrypt.EnhancedVerify(user.Password, userLogin.Password);
            if (hasPassword == false)
            {
                throw new BadRequestException("invalid username/password");
            }
            string accessToken = GenerateToken(userLogin);
            AuthResponse userResponse = new AuthResponse(userLogin.Id, userLogin.Email, userLogin.Fullname, accessToken);
            return userResponse;
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(appSettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(appSettings.Expires)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var accessToken = jwtTokenHandle.CreateToken(tokenDescription);
            return jwtTokenHandle.WriteToken(accessToken);
        }
    }
}
