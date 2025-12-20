using FreelanceMarketplaceService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplaceService.Infrastructure.Data.Context
{
    public class MarketplaceDbContext : DbContext
    {
        public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<PortfolioItem> PortfolioItems { get; set; }
        public DbSet<ClientReview> ClientReviews { get; set; }
        public DbSet<FreelancerReview> FreelancerReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Budget).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.SkillLevel).HasMaxLength(50);

                entity.HasOne(p => p.Client)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(p => p.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.AssignedFreelancer)
                    .WithMany(f => f.AssignedProjects)
                    .HasForeignKey(p => p.AssignedFreelancerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Client configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CompanyName).HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.TotalSpent).HasPrecision(18, 2);
            });

            // Freelancer configuration
            modelBuilder.Entity<Freelancer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
                entity.Property(e => e.Rating).HasPrecision(3, 2);
            });

            // Bid configuration
            modelBuilder.Entity<Bid>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(b => b.Project)
                    .WithMany(p => p.Bids)
                    .HasForeignKey(b => b.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Freelancer)
                    .WithMany(f => f.Bids)
                    .HasForeignKey(b => b.FreelancerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure one freelancer can bid once per project
                entity.HasIndex(b => new { b.ProjectId, b.FreelancerId }).IsUnique();
            });

            // Portfolio configuration
            modelBuilder.Entity<PortfolioItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

                entity.HasOne(p => p.Freelancer)
                    .WithMany(f => f.Portfolio)
                    .HasForeignKey(p => p.FreelancerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}