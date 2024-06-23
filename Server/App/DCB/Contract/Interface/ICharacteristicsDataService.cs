using Contract.Model;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Contract.Interface
{
    public interface ICharacteristicsDataService
    {
        Task<Response<Characteristics>> GetAsync(Guid id, Guid userId, CancellationToken token);
        Task<Response<EntityList<Characteristics>>> GetAsync(CharacteristicsFilter filter, Guid userId, CancellationToken token);
        Task<Response<Characteristics>> UpdateAsync(CharacteristicsUpdater model, Guid userId, CancellationToken token);                
    }
}
