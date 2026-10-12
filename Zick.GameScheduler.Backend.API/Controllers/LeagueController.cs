using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.API.Utils;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

// TODO: Logging

[ApiController]
[Route("[controller]")]
public class LeagueController(ILogger<LeagueController> logger,
    ILeagueService leagueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<LeagueDTO>> GetLeagues()
    {
        try
        {
            var leagues = await leagueService.GetLeagues();
            logger.LogInformation("[GET /league]: sent {leagueCount} leagues to {ip}", 
                leagues.Count,
                HttpContext.Connection.RemoteIpAddress);
            return Ok(leagues);
        }
        catch (Exception e)
        {
            var error = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(error.Code, error.Message);
        }
    }
}