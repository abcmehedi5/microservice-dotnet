using System.ComponentModel.DataAnnotations;

namespace JobPortalService.Application.DTOs
{
    public class CreateJobDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal? Salary { get; set; }

        public string Location { get; set; }

        [Required]
        public string JobType { get; set; } // Full-time, Part-time, Contract

        [Required]
        public Guid CompanyId { get; set; }
    }

    public class UpdateJobDto
    {
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }
        public decimal? Salary { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Status { get; set; }
    }

    public class JobResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Salary { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }

    public class CreateApplicationDto
    {
        [Required]
        public Guid ApplicantId { get; set; }

        public string CoverLetter { get; set; }

        [Required]
        public string ResumeUrl { get; set; }
    }
}