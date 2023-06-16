using Common.Utilities;
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

        public async Task AddUser(string password, User user, CancellationToken cancellationToken)
        {
            user.PasswordHash = SecurityHelper.HashPasswordSHA256(password);
            await AddAsync(user, cancellationToken);
        }

        public Task UpdateSecurityStamp(User user, CancellationToken cancellationToken)
        {
            user.SecurityStamp = Guid.NewGuid();
            return base.UpdateAsync(user, cancellationToken);
        }
    }
}
