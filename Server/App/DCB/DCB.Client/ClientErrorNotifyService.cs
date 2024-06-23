using DCB.Client.Services.Services;
using DCB.Common;

namespace DCB.Client
{
    public class ClientErrorNotifyService(HttpClientService httpClientService) : IDisposable, IClientErrorNotifyService
    {
        private readonly HttpClientService _httpClientService = httpClientService;

        public async Task Send(string message, MessageLevelEnum? level = MessageLevelEnum.Error, string? title = null)
        {
            await _httpClientService.SendErrorMessage(message, level, title);
        }       

        public void Dispose()
        {
            
        }
    }
}