using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserManager<RacingUserIdentity> userManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegistrationBody data)
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
            return Ok();
        return BadRequest(result.Errors);
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