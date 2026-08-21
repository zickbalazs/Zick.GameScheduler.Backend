namespace Zick.GameScheduler.Backend.Dashboard.Models;

public class NavMenuGroup
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public IList<NavMenuItem> Links { get; set; } = [];
}