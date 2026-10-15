using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Data.Dashboard.Forms;

public class AddRacingClassForm
{
    public IList<CarDTO> Cars { get; set; }= [];
    public string Identifier { get; set; } = "";
    public string? Color { get; set; }
    public string? Name { get; set; }
}