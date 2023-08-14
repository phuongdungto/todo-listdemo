using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sieve.Models;
using Sieve.Services;

using todo.Dto;
using todo.Repositories;

namespace todo.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            _userRepository = userRepository;
            _sieveProcessor = sieveProcessor;
            _mapper = mapper;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser(CreateUsersDto user)
        {
            var newUser = await _userRepository.CreateUser(user);
            var userDto = _mapper.Map<UserDto>(newUser);
            return Ok(userDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userRepository.DeleteUser(id);

            return Ok("Deleted user success");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUsersDto input)
        {
            var user = await _userRepository.UpdateUser(id, input);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userRepository.GetUser(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] SieveModel model)
        {
            var users = await _userRepository.GetUsers();
            var pageSize = model.PageSize;
            var page = model.Page;
            model.PageSize = null;
            model.Page = null;
            var totalRecord = _sieveProcessor.Apply(model, users).Count();
            model.PageSize = pageSize;
            model.Page = page;
            users = _sieveProcessor.Apply(model, users);
            var userDto = _mapper.Map<List<UserDto>>(users.ToList());
            if (model.PageSize == null)
            {
                model.PageSize = totalRecord;
            }
            return Ok(new GetUsersResponse
            {
                totalPages = (int)Math.Ceiling((double)totalRecord / (double)model.PageSize),
                Users = userDto.ToList()
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthDto user)
        {
            var userLogin = await _userRepository.Login(user);
            return Ok(userLogin);
        }
    }
}
