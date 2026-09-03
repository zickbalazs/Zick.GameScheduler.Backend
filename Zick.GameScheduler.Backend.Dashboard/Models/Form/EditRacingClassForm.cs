using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Backend.Dashboard.Models.Form;

public class EditRacingClassForm
{
    public string Identifier { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public IList<CarDTO> Cars { get; set; } = [];
}