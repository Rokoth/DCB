using Contract.Model;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Contract.Interface
{
    public interface IPersonDataService
    {
        Task<Response<Person>> GetAsync(Guid id, Guid userId, CancellationToken token);
        Task<Response<EntityList<Person>>> GetAsync(PersonFilter filter, Guid userId, CancellationToken token);
        Task<Response<Person>> UpdateAsync(PersonUpdater model, Guid userId, CancellationToken token);

        Task<Response<Person>> AddAsync(PersonCreator model, Guid userId, CancellationToken token);
    }
}
