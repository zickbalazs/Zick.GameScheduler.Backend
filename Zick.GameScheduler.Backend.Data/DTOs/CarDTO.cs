namespace Zick.GameScheduler.Backend.Data.DTOs;

public class CarDTO : IEquatable<CarDTO>
{
    public Guid Id { get; set; }
    public string? Manufacturer { get; set; }
    public string Model { get; set; }
    public string FolderName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CarDTO)obj);
    }

    public bool Equals(CarDTO? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Manufacturer, Model, FolderName);
    }
}