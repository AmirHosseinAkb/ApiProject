using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Dtos;

namespace Server.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ApiResult<List<User>>> Get(CancellationToken cancellationToken)
        {
            var users=_userRepository.TableNoTracking;
            return Ok(users.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ApiResult<User>> Get(int id,CancellationToken cancellationToken)
        {
            var user =await _userRepository.GetByIdAsync(cancellationToken,id);
            if(user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ApiResult> Create(UserDto user, CancellationToken cancellationToken)
        {
            if (_userRepository.IsExist(u => u.UserName == user.UserName))
                return BadRequest("این کاربر از قبل وجود دارد");
            var newUser = new User()
            {
                UserName = user.UserName,
                Age = user.Age,
                FullName = user.FullName,
                Gender = user.Gender,
            };
            await _userRepository.AddUser(user.Password, newUser, cancellationToken);
            return Ok();
        }

        [HttpPut]
        public async Task<ApiResult> Delete(int id,CancellationToken cancellationToken)
        {
            var user =await _userRepository.GetByIdAsync(cancellationToken,id);
            if (user == null)
                return NotFound("کاربر مورد نظر یافت نشد");
            await _userRepository.DeleteAsync(user, cancellationToken);
            return Ok();
        }
    }
}
