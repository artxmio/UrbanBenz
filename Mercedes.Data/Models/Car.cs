using Mercedes.Data.Enums;

namespace Mercedes.Data.Models;

public class Car
{
    public Guid Id { get; set; }
    public string Model { get; set; } = null!;
    public CarType Type { get; set; } = CarType.Unknown;
    public CarClass Class { get; set; } = CarClass.Unknown;
    public CarEngine Engine { get; set; } = CarEngine.Unknown;
    public int Year { get; set; }
    public string Color { get; set; } = null!;
    public decimal Price { get; set; }
    public string VinNumber { get; set; } = null!;
    public bool IsAviable { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CarImage> Images { get; set; } = [];
}
