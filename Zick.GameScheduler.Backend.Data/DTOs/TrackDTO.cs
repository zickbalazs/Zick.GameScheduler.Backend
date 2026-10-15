namespace Zick.GameScheduler.Backend.Data.DTOs;

public class TrackDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? CountryCode { get; set; }
    public int MaxCapacity { get; set; }
    public string Folder { get; set; }
}