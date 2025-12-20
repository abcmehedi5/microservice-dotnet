using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplaceService.Application.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Budget { get; set; }

        public string Currency { get; set; } = "USD";

        [Required]
        public string SkillLevel { get; set; }

        public string[] RequiredSkills { get; set; } = Array.Empty<string>();

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public Guid ClientId { get; set; }
    }

    public class UpdateProjectDto
    {
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Range(1, 1000000)]
        public decimal? Budget { get; set; }

        public string SkillLevel { get; set; }

        public string[] RequiredSkills { get; set; }

        public DateTime? Deadline { get; set; }

        public string Status { get; set; }
    }

    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string SkillLevel { get; set; }
        public string[] RequiredSkills { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? AssignedFreelancerId { get; set; }
        public string AssignedFreelancerName { get; set; }
        public int BidCount { get; set; }
    }

    public class CreateBidDto
    {
        [Required]
        public Guid FreelancerId { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public string Proposal { get; set; }

        [Required]
        [Range(1, 365)]
        public int DeliveryDays { get; set; }
    }

    public class BidResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid FreelancerId { get; set; }
        public string FreelancerName { get; set; }
        public decimal Amount { get; set; }
        public string Proposal { get; set; }
        public int DeliveryDays { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}