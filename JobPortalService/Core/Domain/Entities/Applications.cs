namespace JobPortalService.Core.Domain.Entities
{
    public class Applications
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid JobId { get; set; }
        public Guid ApplicantId { get; set; }
        public string CoverLetter { get; set; }
        public string ResumeUrl { get; set; }
        public string Status { get; set; } = "Applied";
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Job Job { get; set; }
    }
}
