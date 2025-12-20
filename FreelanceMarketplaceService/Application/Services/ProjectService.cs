using AutoMapper;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Domain.Entities;
using FreelanceMarketplaceService.Core.Interfaces;
using FreelanceMarketplaceService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplaceService.Application.Services
{
    public class ProjectService(MarketplaceDbContext context, IMapper mapper, ILogger<ProjectService> logger) : IProjectService
    {
        private readonly MarketplaceDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProjectService> _logger = logger;

        public async Task<Project> GetProjectByIdAsync(Guid projectId)
        {
            _logger.LogInformation($"Getting project with ID: {projectId}");

            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.AssignedFreelancer)
                .Include(p => p.Bids)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            return project;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            _logger.LogInformation("Getting all projects");

            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.AssignedFreelancer)
                .Where(p => p.Status == "Open")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId)
        {
            _logger.LogInformation($"Getting projects for client ID: {clientId}");

            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.AssignedFreelancer)
                .Where(p => p.ClientId == clientId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status)
        {
            _logger.LogInformation($"Getting projects with status: {status}");

            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.AssignedFreelancer)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project> CreateProjectAsync(CreateProjectDto projectDto)
        {
            _logger.LogInformation($"Creating new project: {projectDto.Title}");

            // Check if client exists
            var client = await _context.Clients.FindAsync(projectDto.ClientId);
            if (client == null)
                throw new KeyNotFoundException($"Client with ID {projectDto.ClientId} not found");

            var project = new Project
            {
                Title = projectDto.Title,
                Description = projectDto.Description,
                Budget = projectDto.Budget,
                Currency = projectDto.Currency,
                SkillLevel = projectDto.SkillLevel,
                RequiredSkills = projectDto.RequiredSkills,
                Deadline = projectDto.Deadline,
                ClientId = projectDto.ClientId,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return project;
        }

        public async Task<Project> UpdateProjectAsync(Guid projectId, UpdateProjectDto projectDto)
        {
            _logger.LogInformation($"Updating project with ID: {projectId}");

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(projectDto.Title))
                project.Title = projectDto.Title;

            if (!string.IsNullOrWhiteSpace(projectDto.Description))
                project.Description = projectDto.Description;

            if (projectDto.Budget.HasValue)
                project.Budget = projectDto.Budget.Value;

            if (!string.IsNullOrWhiteSpace(projectDto.SkillLevel))
                project.SkillLevel = projectDto.SkillLevel;

            if (projectDto.RequiredSkills != null)
                project.RequiredSkills = projectDto.RequiredSkills;

            if (projectDto.Deadline.HasValue)
                project.Deadline = projectDto.Deadline.Value;

            if (!string.IsNullOrWhiteSpace(projectDto.Status))
                project.Status = projectDto.Status;

            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> CloseProjectAsync(Guid projectId)
        {
            _logger.LogInformation($"Closing project with ID: {projectId}");

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            project.Status = "Closed";
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Bid> SubmitBidAsync(Guid projectId, CreateBidDto bidDto)
        {
            _logger.LogInformation($"Submitting bid for project ID: {projectId} by freelancer: {bidDto.FreelancerId}");

            // Check if project exists and is open
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.Status != "Open")
                throw new KeyNotFoundException($"Open project with ID {projectId} not found");

            // Check if freelancer exists
            var freelancer = await _context.Freelancers.FindAsync(bidDto.FreelancerId);
            if (freelancer == null)
                throw new KeyNotFoundException($"Freelancer with ID {bidDto.FreelancerId} not found");

            // Check if already bid
            var existingBid = await _context.Bids
                .FirstOrDefaultAsync(b => b.ProjectId == projectId && b.FreelancerId == bidDto.FreelancerId);

            if (existingBid != null)
                throw new InvalidOperationException($"Freelancer {bidDto.FreelancerId} has already bid on project {projectId}");

            var bid = new Bid
            {
                ProjectId = projectId,
                FreelancerId = bidDto.FreelancerId,
                Amount = bidDto.Amount,
                Proposal = bidDto.Proposal,
                DeliveryDays = bidDto.DeliveryDays,
                Status = "Submitted",
                SubmittedAt = DateTime.UtcNow
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return bid;
        }

        public async Task<Bid> AcceptBidAsync(Guid bidId)
        {
            _logger.LogInformation($"Accepting bid with ID: {bidId}");

            var bid = await _context.Bids
                .Include(b => b.Project)
                .FirstOrDefaultAsync(b => b.Id == bidId);

            if (bid == null)
                throw new KeyNotFoundException($"Bid with ID {bidId} not found");

            if (bid.Project.Status != "Open")
                throw new InvalidOperationException($"Project {bid.ProjectId} is not open for bidding");

            // Update bid status
            bid.Status = "Accepted";
            bid.AcceptedAt = DateTime.UtcNow;

            // Update project
            bid.Project.Status = "InProgress";
            bid.Project.AssignedFreelancerId = bid.FreelancerId;
            bid.Project.UpdatedAt = DateTime.UtcNow;

            // Reject all other bids for this project
            var otherBids = await _context.Bids
                .Where(b => b.ProjectId == bid.ProjectId && b.Id != bidId)
                .ToListAsync();

            foreach (var otherBid in otherBids)
            {
                otherBid.Status = "Rejected";
            }

            await _context.SaveChangesAsync();
            return bid;
        }

        public async Task<IEnumerable<Bid>> GetProjectBidsAsync(Guid projectId)
        {
            _logger.LogInformation($"Getting bids for project ID: {projectId}");

            return await _context.Bids
                .Include(b => b.Freelancer)
                .Where(b => b.ProjectId == projectId)
                .OrderByDescending(b => b.SubmittedAt)
                .ToListAsync();
        }
    }
}