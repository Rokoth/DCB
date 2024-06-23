using System.Threading.Tasks;
using System;
using Contract.Model;
using System.Threading;

namespace Contract.Interface
{
    public interface IMoveService
    {
        Task<Response<Location>> MoveNorth(Guid personId, bool isPerson, Guid? userId, CancellationToken token);
                
    }
}
