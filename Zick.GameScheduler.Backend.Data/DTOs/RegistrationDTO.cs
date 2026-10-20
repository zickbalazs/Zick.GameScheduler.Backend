namespace Zick.GameScheduler.Backend.Data.DTOs;

public class RegistrationDTO
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Guid SessionId { get; set; }
    public bool Locked { get; set; }
}