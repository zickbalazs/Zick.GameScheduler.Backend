using Microsoft.AspNetCore.Mvc;
using Zick.GameScheduler.Backend.API.Utils;
using Zick.GameScheduler.Backend.Data.DTOs;
using Zick.GameScheduler.Services.ServicesAPI;

namespace Zick.GameScheduler.Backend.API.Controllers;

// TODO: Logging


[ApiController]
[Route("[controller]")]
public class TrackController(ILogger<TrackController> logger, ITrackService trackService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IList<TrackDTO>>> GetTracks()
    {
        try
        {
            var tracks = await trackService.GetTracks();
            logger.LogInformation("[GET: /tracks]: sent {trackCount} for host {ip}",
                tracks.Count,
                HttpContext.Connection.RemoteIpAddress);
            return Ok(tracks);
        }
        catch (Exception e)
        {
            var error = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(error.Code, error.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackDTO>> GetTrack(Guid id)
    {
        try
        {
            var track = await trackService.GetTrack(id);
            logger.LogInformation("[GET: /tracks/id]: sent track for host {ip}", HttpContext.Connection.RemoteIpAddress);
            return Ok(track);
        }
        catch (Exception e)
        {
            var error = ResponseUtilsFactory.Create(logger).HandleFirstMethodException(e);
            return StatusCode(error.Code, error.Message);
        }
    }
}