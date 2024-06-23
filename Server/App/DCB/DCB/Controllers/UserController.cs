using Contract.Interface;
using Contract.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DCB.Controllers
{
    [ApiController, Authorize, Produces("application/json")]
    [Route("api/user")]
    public class UserController(IUserDataService userDataService, ILogger<UserController> logger) : Controller
    {
        private IUserDataService UserDataService { get; } = userDataService;
        private ILogger<UserController> Logger { get; } = logger;
                
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList(UserFilter filter)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var result = await UserDataService.GetAsync(filter, userId, source.Token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении списка пользователей");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var result = await UserDataService.GetAsync(id, userId, source.Token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении пользователя");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
