namespace FreelanceMarketplaceService.Core.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Open"; // Open, InProgress, Completed, Closed
    public string SkillLevel { get; set; } // Beginner, Intermediate, Expert
    public string[] RequiredSkills { get; set; } = Array.Empty<string>();
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public Guid ClientId { get; set; }
    public Guid? AssignedFreelancerId { get; set; }

    // Navigation properties
    public Client Client { get; set; }
    public Freelancer? AssignedFreelancer { get; set; }
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}