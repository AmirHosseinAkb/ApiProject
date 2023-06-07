using Data.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
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
            return Ok(user);
        }
    }
}
