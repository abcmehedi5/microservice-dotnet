namespace FreelanceMarketplaceService.Core.Domain.Entities
{
    public class ClientReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; }
        public Guid ReviewerId { get; set; } // Freelancer who reviewed
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Client Client { get; set; }
    }

    public class FreelancerReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FreelancerId { get; set; }
        public Guid ReviewerId { get; set; } // Client who reviewed
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Freelancer Freelancer { get; set; }
    }
}