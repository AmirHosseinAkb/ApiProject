using Data.Contracts;
using Entities.User;

namespace Data.Repositories
{
    public class UserRepository:BaseRepository<User>,IUserRepository
    {
        private readonly MyApiContext _context;
        public UserRepository(MyApiContext context) : base(context)
        {
            _context = context;
        }
    }
}
