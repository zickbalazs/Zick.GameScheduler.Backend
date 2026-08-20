namespace Zick.GameScheduler.Backend.Data.DTOs;

public class RacingClassDTO
{
    public string Abbreviation { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public IList<CarDTO> Cars { get; set; } = [];
}