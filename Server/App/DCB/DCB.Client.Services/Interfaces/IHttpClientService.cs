using DCB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCB.Client.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task SendErrorMessage(string message, MessageLevelEnum? level, string? title);
    }
}
