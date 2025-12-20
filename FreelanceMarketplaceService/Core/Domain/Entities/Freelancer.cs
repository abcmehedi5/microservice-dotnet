namespace FreelanceMarketplaceService.Core.Domain.Entities
{
    public class Freelancer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } // Reference to Auth Service
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal HourlyRate { get; set; }
        public string[] Skills { get; set; } = Array.Empty<string>();
        public string Country { get; set; }
        public decimal Rating { get; set; }
        public int TotalProjects { get; set; }
        public int SuccessRate { get; set; } // Percentage
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<FreelancerReview> Reviews { get; set; } = new List<FreelancerReview>();
        public ICollection<PortfolioItem> Portfolio { get; set; } = new List<PortfolioItem>();
    }
}