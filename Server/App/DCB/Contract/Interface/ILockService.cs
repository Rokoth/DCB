using System.Threading.Tasks;
using System;

namespace Contract.Interface
{
    public interface ILockService
    {
        Task<bool> Lock(Guid id);

        Task Unlock(Guid id);
    }
}
