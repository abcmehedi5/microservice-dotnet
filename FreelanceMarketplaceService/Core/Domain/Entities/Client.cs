namespace FreelanceMarketplaceService.Core.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } // Reference to Auth Service
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalProjects { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ClientReview> Reviews { get; set; } = new List<ClientReview>();
    }
}