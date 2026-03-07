namespace UrbanBenz.Data.Models;

public class CarImage
{
    public Guid Id { get; set; }
    public string Path { get; set; } = null!;
    
    public Guid CarId { get; set; }
    public Car Car { get; set; } = null!;

    public bool IsMain { get; set; }
}
