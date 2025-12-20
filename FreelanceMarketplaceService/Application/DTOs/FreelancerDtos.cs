using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplaceService.Application.DTOs
{
    public class CreateFreelancerDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public decimal HourlyRate { get; set; }

        public string[] Skills { get; set; } = Array.Empty<string>();

        public string Country { get; set; }

        public string ProfileImageUrl { get; set; }
    }

    public class UpdateFreelancerDto
    {
        [StringLength(200)]
        public string FullName { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Range(1, 1000)]
        public decimal? HourlyRate { get; set; }

        public string[] Skills { get; set; }

        public string Country { get; set; }

        public string ProfileImageUrl { get; set; }
    }

    public class FreelancerResponseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal HourlyRate { get; set; }
        public string[] Skills { get; set; }
        public string Country { get; set; }
        public decimal Rating { get; set; }
        public int TotalProjects { get; set; }
        public int SuccessRate { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PortfolioCount { get; set; }
    }

    public class CreatePortfolioItemDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string[] Technologies { get; set; } = Array.Empty<string>();

        public string ProjectUrl { get; set; }

        public string ImageUrl { get; set; }
    }
}