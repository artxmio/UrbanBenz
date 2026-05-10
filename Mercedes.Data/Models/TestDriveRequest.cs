using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mercedes.Data.Models;

public class TestDriveRequest
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CarId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTime RequestedDate { get; set; }

    public DateTime? ConfirmedDate { get; set; }

    [Required]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required]
    public TestDriveStatus Status { get; set; } = TestDriveStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(CarId))]
    public Car? Car { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}

public enum TestDriveStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}