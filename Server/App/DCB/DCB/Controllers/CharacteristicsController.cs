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
    [Route("api/characteristics")]
    public class CharacteristicsController(ICharacteristicsDataService characteristicsDataService, ILogger<CharacteristicsController> logger) : Controller
    {
        private ICharacteristicsDataService CharacteristicsDataService { get; } = characteristicsDataService;
        private ILogger<CharacteristicsController> Logger { get; } = logger;
                
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList(CharacteristicsFilter filter)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var result = await CharacteristicsDataService.GetAsync(filter, userId, source.Token);
                if (result.IsSuccess)
                    return Ok(result.Value);

                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении списка характеристик");
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
                var result = await CharacteristicsDataService.GetAsync(id, userId, source.Token);
                if (result.IsSuccess)
                    return Ok(result.Value);

                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при получении характеристики");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
    }
}
