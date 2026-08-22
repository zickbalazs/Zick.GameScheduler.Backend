namespace Zick.GameScheduler.Backend.Data.DTOs;

public class CarDTO
{
    public Guid Id { get; set; }
    public string? Manufacturer { get; set; }
    public string Model { get; set; }
    public string FolderName { get; set; }
}