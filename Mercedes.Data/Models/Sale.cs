using System.ComponentModel.DataAnnotations;
using Mercedes.Data.Enums;

namespace Mercedes.Data.Models;

public class Sale
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid CarId { get; set; }
    public Car Car { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
}