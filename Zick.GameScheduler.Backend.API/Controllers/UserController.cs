using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger,
    UserManager<RacingUserIdentity> userManager,
    SignInManager<RacingUserIdentity> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegistrationBody data)
    {
        try
        {
            var result = await userManager.CreateAsync(new()
            {
                UserName = data.UserName,
                Email = data.Email,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                SteamId = data.SteamID
            }, data.Password);

            if (result.Succeeded)
            {
                logger.LogInformation("[/register]: new user registered to db from {ip}",
                    HttpContext.Connection.RemoteIpAddress);
                return Ok();
            }

            result.Errors.ToList().ForEach(x => ModelState.AddModelError(x.Code, x.Description));
            return ValidationProblem();
        }
        catch (Exception e)
        {
            logger.LogError("[/register]: an error has occured with:\nmessage: {message}\nstack: {stack}",
                e.Message,
                e.StackTrace);
            return StatusCode(500, "A server error has occured, contact the administrators!");
        }
    }


    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserData>> GetUserData()
    {
        if (User.Identity == null || User.Identity.Name == null)
            return Unauthorized();

        var user = await userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
            return Unauthorized();

        return Ok(new UserData()
        {
          Name  = user!.UserName!,
          Email = user!.Email!
        });

    }
    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}


public class RegistrationBody
{
    [EmailAddress]
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SteamID { get; set; }
}

public class UserData
{
    public string Email { get; set; }
    public string Name { get; set; }
}