using AutoMapper;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Domain.Entities;
using FreelanceMarketplaceService.Core.Interfaces;
using FreelanceMarketplaceService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplaceService.Application.Services
{
    public class FreelancerService : IFreelancerService
    {
        private readonly MarketplaceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<FreelancerService> _logger;

        public FreelancerService(MarketplaceDbContext context, IMapper mapper, ILogger<FreelancerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Freelancer> GetFreelancerByIdAsync(Guid freelancerId)
        {
            _logger.LogInformation($"Getting freelancer with ID: {freelancerId}");

            var freelancer = await _context.Freelancers
                .Include(f => f.Portfolio)
                .FirstOrDefaultAsync(f => f.Id == freelancerId);

            if (freelancer == null)
                throw new KeyNotFoundException($"Freelancer with ID {freelancerId} not found");

            return freelancer;
        }

        public async Task<Freelancer> GetFreelancerByUserIdAsync(string userId)
        {
            _logger.LogInformation($"Getting freelancer with User ID: {userId}");

            var freelancer = await _context.Freelancers
                .Include(f => f.Portfolio)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (freelancer == null)
                throw new KeyNotFoundException($"Freelancer with User ID {userId} not found");

            return freelancer;
        }

        public async Task<IEnumerable<Freelancer>> SearchFreelancersAsync(string skill, string country, decimal? maxHourlyRate)
        {
            _logger.LogInformation($"Searching freelancers - Skill: {skill}, Country: {country}, Max Rate: {maxHourlyRate}");

            var query = _context.Freelancers
                .Include(f => f.Portfolio)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(skill))
                query = query.Where(f => f.Skills.Contains(skill));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(f => f.Country == country);

            if (maxHourlyRate.HasValue)
                query = query.Where(f => f.HourlyRate <= maxHourlyRate.Value);

            return await query
                .OrderByDescending(f => f.Rating)
                .ThenByDescending(f => f.SuccessRate)
                .ToListAsync();
        }

        public async Task<Freelancer> CreateFreelancerAsync(CreateFreelancerDto freelancerDto)
        {
            _logger.LogInformation($"Creating new freelancer: {freelancerDto.FullName}");

            // Check if user already has a freelancer profile
            var existingFreelancer = await _context.Freelancers
                .FirstOrDefaultAsync(f => f.UserId == freelancerDto.UserId);

            if (existingFreelancer != null)
                throw new InvalidOperationException($"Freelancer profile already exists for user {freelancerDto.UserId}");

            var freelancer = new Freelancer
            {
                UserId = freelancerDto.UserId,
                FullName = freelancerDto.FullName,
                Title = freelancerDto.Title,
                Description = freelancerDto.Description,
                HourlyRate = freelancerDto.HourlyRate,
                Skills = freelancerDto.Skills,
                Country = freelancerDto.Country,
                ProfileImageUrl = freelancerDto.ProfileImageUrl,
                Rating = 0,
                TotalProjects = 0,
                SuccessRate = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Freelancers.Add(freelancer);
            await _context.SaveChangesAsync();

            return freelancer;
        }

        public async Task<Freelancer> UpdateFreelancerAsync(Guid freelancerId, UpdateFreelancerDto freelancerDto)
        {
            _logger.LogInformation($"Updating freelancer with ID: {freelancerId}");

            var freelancer = await _context.Freelancers.FindAsync(freelancerId);
            if (freelancer == null)
                throw new KeyNotFoundException($"Freelancer with ID {freelancerId} not found");

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(freelancerDto.FullName))
                freelancer.FullName = freelancerDto.FullName;

            if (!string.IsNullOrWhiteSpace(freelancerDto.Title))
                freelancer.Title = freelancerDto.Title;

            if (!string.IsNullOrWhiteSpace(freelancerDto.Description))
                freelancer.Description = freelancerDto.Description;

            if (freelancerDto.HourlyRate.HasValue)
                freelancer.HourlyRate = freelancerDto.HourlyRate.Value;

            if (freelancerDto.Skills != null)
                freelancer.Skills = freelancerDto.Skills;

            if (!string.IsNullOrWhiteSpace(freelancerDto.Country))
                freelancer.Country = freelancerDto.Country;

            if (!string.IsNullOrWhiteSpace(freelancerDto.ProfileImageUrl))
                freelancer.ProfileImageUrl = freelancerDto.ProfileImageUrl;

            await _context.SaveChangesAsync();
            return freelancer;
        }

        public async Task<PortfolioItem> AddPortfolioItemAsync(Guid freelancerId, CreatePortfolioItemDto portfolioDto)
        {
            _logger.LogInformation($"Adding portfolio item for freelancer ID: {freelancerId}");

            var freelancer = await _context.Freelancers.FindAsync(freelancerId);
            if (freelancer == null)
                throw new KeyNotFoundException($"Freelancer with ID {freelancerId} not found");

            var portfolioItem = new PortfolioItem
            {
                FreelancerId = freelancerId,
                Title = portfolioDto.Title,
                Description = portfolioDto.Description,
                Technologies = portfolioDto.Technologies,
                ProjectUrl = portfolioDto.ProjectUrl,
                ImageUrl = portfolioDto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.PortfolioItems.Add(portfolioItem);
            await _context.SaveChangesAsync();

            return portfolioItem;
        }

        public async Task<IEnumerable<PortfolioItem>> GetFreelancerPortfolioAsync(Guid freelancerId)
        {
            _logger.LogInformation($"Getting portfolio for freelancer ID: {freelancerId}");

            return await _context.PortfolioItems
                .Where(p => p.FreelancerId == freelancerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}