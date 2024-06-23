using DCB.Client.Services.Interfaces;
using DCB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCB.Client.Services.Services
{
    public class HttpClientService : IHttpClientService
    {
        public Task SendErrorMessage(string message, MessageLevelEnum? level, string? title)
        {
            throw new NotImplementedException();
        }
    }
}
