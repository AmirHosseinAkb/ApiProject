using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var users=_userRepository.TableNoTracking;
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var user =await _userRepository.GetByIdAsync(cancellationToken,id);
            if(user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDto user, CancellationToken cancellationToken)
        {
            var newUser = new User()
            {
                UserName = user.UserName,
                Age = user.Age,
                FullName = user.FullName,
                Gender = user.Gender,
            };
            await _userRepository.AddUser(user.Password, newUser, cancellationToken);
            return Ok(user);
        }
    }
}
