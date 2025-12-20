using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Domain.Entities;

namespace FreelanceMarketplaceService.Core.Interfaces
{
    public interface IFreelancerService
    {
        Task<Freelancer> GetFreelancerByIdAsync(Guid freelancerId);
        Task<Freelancer> GetFreelancerByUserIdAsync(string userId);
        Task<IEnumerable<Freelancer>> SearchFreelancersAsync(string skill, string country, decimal? maxHourlyRate);
        Task<Freelancer> CreateFreelancerAsync(CreateFreelancerDto freelancerDto);
        Task<Freelancer> UpdateFreelancerAsync(Guid freelancerId, UpdateFreelancerDto freelancerDto);
        Task<PortfolioItem> AddPortfolioItemAsync(Guid freelancerId, CreatePortfolioItemDto portfolioDto);
        Task<IEnumerable<PortfolioItem>> GetFreelancerPortfolioAsync(Guid freelancerId);
    }
}