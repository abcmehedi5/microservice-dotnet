namespace JobPortalService.Core.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Navigation
        public ICollection<Job> Jobs { get; set; }
    }
}
