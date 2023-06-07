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

        public IActionResult Get()
        {
            return null;
        }

    }
}
