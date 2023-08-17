using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using todo.Data;
using todo.Dto;
using todo.Enums;
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
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        private const int salt = 10;
        public UserService(TodoDbContext context, IMapper _mapper, IOptionsMonitor<AppSettings> optionsMonitor, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = _mapper;
            this.appSettings = optionsMonitor.CurrentValue;
            this.userManager = userManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
        }
        async Task<bool> IUserRepository.CreateUser(CreateUsersDto user)
        {
            var exist = await userManager.FindByEmailAsync(user.Email);
            if (exist != null)
            {
                throw new BadHttpRequestException("Email already existed ");
            }
            var identityUser = new User()
            {
                UserName = user.Email,
                Email = user.Email,
                Fullname = user.Fullname
            };
            var result = await userManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded)
            {
                throw new BadHttpRequestException("User creation failed! Please check user details and try again");
            }
            if (!await roleManager.RoleExistsAsync(Roles.admin.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.admin.ToString()));
            if (!await roleManager.RoleExistsAsync(Roles.user.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.user.ToString()));
            await userManager.AddToRoleAsync(identityUser, user.Role);
            return result.Succeeded;
        }

        async Task<AuthResponse> IUserRepository.Login(AuthDto input)
        {
            var user = await userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                throw new BadRequestException("invalid username/password");
            }
            if (!await userManager.CheckPasswordAsync(user, input.Password))
            {
                throw new BadRequestException("invalid username/password");
            }
            var listRole = await userManager.GetRolesAsync(user);
            var result = new AuthResponse()
            {
                Id = user.Id,
                Email = user.Email,
                Fullname = user.Fullname,
                AccessToken = GenerateToken(user, listRole[0])
            };
            return result;

        }

        async Task IUserRepository.DeleteUser(string id)
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

        async Task<UserDto> IUserRepository.GetUser(string id)
        {
            var user = await context.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            var listRole = await userManager.GetRolesAsync(user);
            var userDto = mapper.Map<UserDto>(user);
            userDto.Role = listRole[0];
            return userDto;
        }

        async Task<UserDto> IUserRepository.UpdateUser(string id, UpdateUsersDto input)
        {
            var user = await context.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            using var myTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                MapObjects.Assign<User, UpdateUsersDto>(user, input);
                context.Users.Update(user);
                await context.SaveChangesAsync();
                if (input.Role != null)
                {
                    var userRole = (from ur in context.UserRoles
                                    join r in context.Roles on ur.RoleId equals r.Id
                                    where ur.UserId == user.Id
                                    select ur).ToList();
                    context.UserRoles.RemoveRange(userRole);
                    await userManager.AddToRoleAsync(user, input.Role);
                    await context.SaveChangesAsync();
                }
                await myTransaction.CommitAsync();
                var userDto = mapper.Map<UserDto>(user);
                var listRole = await userManager.GetRolesAsync(user);
                userDto.Role = listRole[0];
                return userDto;
            }
            catch (Exception ex)
            {
                await myTransaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }
        async Task<IQueryable<User>> IUserRepository.GetUsers()
        {
            var users = from u in context.Users
                        select u;
            return users;
        }

        private string GenerateToken(User user, string role)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Secret").Value);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id),
                    new Claim("fullname", user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role,role)
                }),
                Issuer = configuration.GetSection("JWT:ValidIssuer").Value,
                Audience = configuration.GetSection("JWT:ValidAudience").Value,
                Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(configuration.GetSection("JWT:Expires").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var accessToken = jwtTokenHandle.CreateToken(tokenDescription);
            return jwtTokenHandle.WriteToken(accessToken);
        }
    }
}
