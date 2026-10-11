using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LeagueController(ILogger<LeagueController> logger,
    ILeagueService leagueService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLeagues()
    {
        try
        {
            return Ok(await leagueService.GetLeagues());
        }
        catch (Exception e)
        {
            ModelState.AddModelError("error", e.Message);
            logger.LogError("error: {message}\nStackTrace: {stack}", e.Message, e.StackTrace);
            return ValidationProblem();
        }
    }
}