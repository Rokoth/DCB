using DCB.Common;

namespace DCB.Client
{
    public interface IClientErrorNotifyService
    {
        void Dispose();
        Task Send(string message, MessageLevelEnum? level = null, string? title = null);
    }
}