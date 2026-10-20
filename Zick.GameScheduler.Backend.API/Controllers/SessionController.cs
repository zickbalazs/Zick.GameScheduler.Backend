using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Zick.GameScheduler.Backend.API.Hubs;
using Zick.GameScheduler.Backend.API.Models;
using Zick.GameScheduler.Backend.API.Utils;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

// TODO: HUB notify

[ApiController]
[Route("[controller]")]
public class SessionController(
    ILogger<SessionController> logger,
    ISessionService sessionService,
    IHubContext<RegistrationHub> hubCtx,
    UserManager<RacingUserIdentity> userManager) : ControllerBase
{
    [HttpGet("league/{leagueId}")]
    public async Task<ActionResult<IList<SessionDTO>>> GetSessionsForLeague(Guid leagueId)
    {
        try
        {
            return Ok(await sessionService.GetSessionsByLeagueId(leagueId));
        }
        catch (Exception e)
        {
            var response = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(response.Code, response);
        }
    }

    [HttpGet("league/available/{leagueId}")]
    public async Task<ActionResult<IEnumerable<SessionDTO>>> GetAvailableSessionsForLeague(Guid leagueId)
    {
        try
        {
            var sessions = await sessionService.GetSessionsByLeagueId(leagueId);
            return Ok(sessions.Where(x=>x.Status == SessionStatus.Waiting));
        }
        catch (Exception e)
        {
            ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(Response.StatusCode, Response);
        }
    }

    [HttpGet("connect/{sessionId}")]
    public async Task<ActionResult> GetSessionConnectionData(Guid sessionId)
    {
        try
        {
            return Ok(await sessionService.GetConnectionForSession(sessionId));
        }
        catch (Exception e)
        {
            var res = Utils.ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(res.Code, res);
        }
    }
    
    [Authorize]
    [HttpGet("registered")]
    public async Task<ActionResult<IEnumerable<SessionDTO>>> GetRegisteredSessions()
    {
        try
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                throw new InvalidOperationException();
            }
            var user = await userManager.FindByNameAsync(User.Identity.Name) ?? throw new InvalidOperationException();
            return Ok(await sessionService.GetRegisteredSessions(user.Id));
        }
        catch (Exception e)
        {
            var res = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(res.Code, res);
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
            return StatusCode(response.Code, response);
        }
    }

    [Authorize]
    [HttpPost("register/{sessionId}")]
    public async Task<IActionResult> RegisterForSession(Guid sessionId, [FromBody] SessionRegisterBody details)
    {
        using (logger.BeginScope("[POST /register/{sessionId}]: ", sessionId))
        {
            try
            {
                string userId = await GetUserSteamId();
                await sessionService.RegisterToSession(sessionId, userId, details.CarId);
                await hubCtx.Clients.Group(sessionId.ToString()).SendAsync("LobbySizeChange",
                    sessionId.ToString(),
                    (await sessionService.GetSession(sessionId)).RegistrationIds.Count);
                logger.LogInformation("Registered user {userId} to session with car: {carId}",
                    userId,
                    details.CarId);
                var response = new
                {
                    Success = true
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                if (e is MethodAccessException or UnauthorizedAccessException or DuplicateNameException)
                {
                    var response = new ExceptionDetail(400, e.Message);
                    return StatusCode(response.Code, response);
                }

                var serverError = new ExceptionDetail(500, e.Message);
                return StatusCode(serverError.Code, serverError);
            }    
        }
    }

    [Authorize]
    [HttpPost("unregister/{sessionId}")]
    public async Task<ActionResult<bool>> UnregisterFromSession(Guid sessionId)
    {
        try
        {
            string userId = await GetUserSteamId();
            await sessionService.UnregisterFromSession(sessionId, userId);
            await hubCtx.Clients.Group(sessionId.ToString()).SendAsync("LobbySizeChange",
                sessionId.ToString(),
                (await sessionService.GetSession(sessionId)).RegistrationIds.Count);
            return Ok(true);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or InvalidOperationException)
                return NotFound();

            if (e is ArgumentException or UnauthorizedAccessException)
                return Unauthorized();
            
            logger.LogError("[POST /unregister/{sessionId}]: unregistration failed with error:\nmessage: {message}\nstack: {stack}",
                sessionId,
                e.Message,
                e.StackTrace);
            
            return StatusCode(500, false);
        }
    }

    [Authorize]
    [HttpPatch("modify/{sessionId}")]
    public async Task<ActionResult<bool>> ModifyRegistrationForSession(Guid sessionId, Guid newCarId)
    {
        throw new NotImplementedException();
    }

    private async Task<string> GetUserSteamId()
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            throw new ArgumentException();
        }
        
        var user = await userManager.FindByNameAsync(User.Identity.Name);

        if (user == null || user.SteamId == null)
        {
            throw new UnauthorizedAccessException();
        }

        return user.Id;
    }
    
}