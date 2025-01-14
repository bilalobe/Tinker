using System.ComponentModel.DataAnnotations;

namespace Tinker.Infrastructure.Security.Compliance.Models;

public class ComplianceReport
{
    public int Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Details { get; set; } = string.Empty;

    [Required]
    public DateTime OrderDate { get; set; }
}