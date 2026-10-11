using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleController(IVehicleService vehicleService) : ControllerBase
{

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetClassVehicles(string classId)
    {
        try
        {
            return Ok(await vehicleService.GetCarsByClass(classId));
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException or InvalidOperationException)
            {
                ModelState.AddModelError("classId", "Class with this abbreviation is not found!");
                return ValidationProblem();
            }
            return StatusCode(500, "A database error has occured!");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        return Ok(await vehicleService.GetCars());
    }
}