using Entities.User;

namespace Data.Contracts
{
    public interface IUserRepository:IRepository<User>
    {
        Task AddUser(string password,User user,CancellationToken  cancellationToken);
        Task UpdateSecurityStamp(User user, CancellationToken cancellationToken);
    }
}
