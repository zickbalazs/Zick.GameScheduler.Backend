using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Services.API;

namespace Zick.GameScheduler.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistrationController(UserManager<RacingUserIdentity> userManager,
    IRegistrationService regService,
    ILogger<RegistrationController> logger) : ControllerBase
{
    [Authorize]
    [HttpDelete("{regId}")]
    public async Task<IActionResult> CancelRegistration(Guid regId)
    {
        try
        {
            string userId = await GetUserId(User);
            await regService.CancelRegistration(regId, userId);
            return Ok();
        }
        catch (Exception e)
        {
            var res = Utils.ResponseUtilsFactory.Create(logger).HandleAuthExceptions(e);
            return StatusCode(res.Code, res);
        }
    }

    private async Task<string> GetUserId(ClaimsPrincipal claims)
    {
        if (claims.Identity == null || !claims.Identity.IsAuthenticated || claims.Identity.Name == null)
            throw new UnauthorizedAccessException("You must be logged in to use this.");

        var user = await userManager.FindByNameAsync(claims.Identity.Name) 
                   ?? throw new UnauthorizedAccessException("You must be logged in to use this.");
        
        return user.Id;
    }
    
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IList<RegistrationDTO>>> GetRegistrations()
    {
        try
        {
            var username = User.Identity?.Name;

            if (username == null || User.Identity == null || !User.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("no authenticated user is found.");
            
            var user = await userManager.FindByNameAsync(username) 
                       ?? throw new ArgumentException("no user is found in db with this name");
            
            var regs = await regService.GetRegistrations(user.Id);
            return Ok(regs);
        }
        catch (Exception e)
        {
            var response = Utils.ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(response.Code, response);
        }
    } 
    
    
}