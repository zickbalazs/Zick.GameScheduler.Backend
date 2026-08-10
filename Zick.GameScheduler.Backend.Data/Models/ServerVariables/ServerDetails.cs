namespace Zick.GameScheduler.Backend.Data.Models.ServerVariables;

public class ServerDetails
{
    public string Name { get; set; }
    
    public string? Password { get; set; }

    public bool SetupCanBeModified { get; set; } = true;
}