using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.API.Utils;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

// TODO: Logging

[ApiController]
[Route("[controller]")]
public class VehicleController(ILogger<VehicleController> logger, IVehicleService vehicleService) : ControllerBase
{

    [HttpGet("class/{classId}")]
    public async Task<ActionResult<CarDTO>> GetClassVehicles(string classId)
    {
        try
        {
            var cars = await vehicleService.GetCarsByClass(classId);
            logger.LogInformation("[GET /class/id]: sent {carCount} cars with class {class} for {ip}", 
                cars.Count,
                classId,
                HttpContext.Connection.RemoteIpAddress);
            return Ok(cars);
        }
        catch (Exception e)
        {
            var error = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(error.Code, error);
        }
    }

    [HttpGet]
    public async Task<ActionResult<CarDTO>> GetVehicles()
    {
        try
        {
            var cars = await vehicleService.GetCars();
            logger.LogInformation("[GET /]: sent {carCount} for {ip}",
                cars.Count,
                HttpContext.Connection.RemoteIpAddress);
            return Ok(cars);
        }
        catch (Exception e)
        {
            var error = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(error.Code, error);
        }
    }
}