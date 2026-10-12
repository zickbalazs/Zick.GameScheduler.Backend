using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.API.Utils;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

// TODO: Logging

[ApiController]
[Route("[controller]")]
public class SessionController(ILogger<SessionController> logger, ISessionService sessionService) : ControllerBase
{
    [HttpGet("league/{leagueId}")]
    public async Task<ActionResult<SessionDTO>> GetSessionsForLeague(Guid leagueId)
    {
        try
        {
            return Ok(await sessionService.GetSessionsByLeagueId(leagueId));
        }
        catch (Exception e)
        {
            var response = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(response.Code, response.Message);
        }
    }
    [HttpGet("byId/{sessionId}")]
    public async Task<ActionResult<SessionDTO>> GetSession(Guid sessionId)
    {
        try
        {
            return Ok(await sessionService.GetSession(sessionId));
        }
        catch (Exception e)
        {
            var response = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(response.Code, e.Message);
        }
    }

    [Authorize]
    [HttpPost("register/{sessionId}")]
    public async Task<ActionResult<bool>> RegisterForSession(Guid sessionId)
    {
        logger.LogInformation("user: {user}", HttpContext.User.Identity?.Name);


        return false;
    }

    [Authorize]
    [HttpPost("unregister/{sessionId}")]
    public async Task<ActionResult<bool>> UnregisterFromSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPatch("modify/{sessionId}")]
    public async Task<ActionResult<bool>> ModifyRegistrationForSession(Guid sessionId, Guid newCarId)
    {
        throw new NotImplementedException();
    }
}