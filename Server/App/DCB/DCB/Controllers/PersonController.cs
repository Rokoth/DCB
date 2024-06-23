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
    [Route("api/person")]
    public class PersonController(IPersonDataService personDataService, ILogger<PersonController> logger) : Controller
    {
        private IPersonDataService PersonDataService { get; } = personDataService;
        private ILogger<PersonController> Logger { get; } = logger;
                
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList(PersonFilter filter)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var result = await PersonDataService.GetAsync(filter, userId, source.Token);
                if (result.IsSuccess)
                    return Ok(result.Value);

                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении списка персонажей");
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
                var result = await PersonDataService.GetAsync(id, userId, source.Token);
                if (result.IsSuccess)
                    return Ok(result.Value);

                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении персонажа");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(PersonCreator creator)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var result = await PersonDataService.AddAsync(creator, userId, source.Token);
                if(result.IsSuccess)
                    return Ok(result.Value);

                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при добавлении персонажа");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
