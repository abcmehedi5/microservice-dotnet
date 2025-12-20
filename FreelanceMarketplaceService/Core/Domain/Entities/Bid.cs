namespace FreelanceMarketplaceService.Core.Domain.Entities
{
    public class Bid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        public Guid FreelancerId { get; set; }
        public decimal Amount { get; set; }
        public string Proposal { get; set; }
        public int DeliveryDays { get; set; }
        public string Status { get; set; } = "Submitted"; // Submitted, Accepted, Rejected
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }

        // Navigation
        public Project Project { get; set; }
        public Freelancer Freelancer { get; set; }
    }
}