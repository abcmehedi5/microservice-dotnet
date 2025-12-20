namespace FreelanceMarketplaceService.Core.Domain.Entities
{
    public class PortfolioItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FreelancerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Technologies { get; set; } = Array.Empty<string>();
        public string ProjectUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Freelancer Freelancer { get; set; }
    }
}