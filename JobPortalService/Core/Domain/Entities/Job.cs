using static System.Net.Mime.MediaTypeNames;

namespace JobPortalService.Core.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal? Salary { get; set; }
    public string Location { get; set; }
    public string JobType { get; set; } // Full-time, Part-time, Contract
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public Guid CompanyId { get; set; }

    // Navigation properties
    public Company Company { get; set; }
    public ICollection<Applications> Applications { get; set; }
}
