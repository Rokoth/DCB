using Contract.Model;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Contract.Interface
{
    public interface IUserDataService
    {
        Task<EntityList<User>> GetAsync(UserFilter filter, Guid userId, CancellationToken token);
        Task<User> GetAsync(Guid id, Guid userId, CancellationToken token);
    }
}
