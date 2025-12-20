using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Domain.Entities;

namespace FreelanceMarketplaceService.Core.Interfaces
{
    public interface IProjectService
    {
        Task<Project> GetProjectByIdAsync(Guid projectId);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId);
        Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status);
        Task<Project> CreateProjectAsync(CreateProjectDto projectDto);
        Task<Project> UpdateProjectAsync(Guid projectId, UpdateProjectDto projectDto);
        Task<bool> CloseProjectAsync(Guid projectId);
        Task<Bid> SubmitBidAsync(Guid projectId, CreateBidDto bidDto);
        Task<Bid> AcceptBidAsync(Guid bidId);
        Task<IEnumerable<Bid>> GetProjectBidsAsync(Guid projectId);
    }
}