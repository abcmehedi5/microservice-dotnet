using Grpc.Core;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Interfaces;
using SharedKernel.gRPC;
using static SharedKernel.gRPC.MarketplaceGrpc;

namespace FreelanceMarketplaceService.API.gRPC
{
    public class MarketplaceGrpcService : MarketplaceGrpcBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<MarketplaceGrpcService> _logger;

        public MarketplaceGrpcService(IProjectService projectService, ILogger<MarketplaceGrpcService> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        public override async Task<ProjectResponse> GetProject(ProjectRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"gRPC GetProject request received for project ID: {request.ProjectId}");

                if (!Guid.TryParse(request.ProjectId, out var projectId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid project ID format"));

                var project = await _projectService.GetProjectByIdAsync(projectId);

                return new ProjectResponse
                {
                    ProjectId = project.Id.ToString(),
                    Title = project.Title,
                    Description = project.Description,
                    Budget = (double)project.Budget,
                    Status = project.Status,
                    ClientId = project.ClientId.ToString()
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Project not found: {request.ProjectId}");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetProject gRPC call for project ID: {request.ProjectId}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<ProjectResponse> CreateProject(CreateProjectRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"gRPC CreateProject request received: {request.Title}");

                if (!Guid.TryParse(request.ClientId, out var clientId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid client ID format"));

                var projectDto = new CreateProjectDto
                {
                    Title = request.Title,
                    Description = request.Description,
                    Budget = (decimal)request.Budget,
                    ClientId = clientId
                };

                var project = await _projectService.CreateProjectAsync(projectDto);

                return new ProjectResponse
                {
                    ProjectId = project.Id.ToString(),
                    Title = project.Title,
                    Description = project.Description,
                    Budget = (double)project.Budget,
                    Status = project.Status,
                    ClientId = project.ClientId.ToString()
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Client not found: {request.ClientId}");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in CreateProject gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<ProjectListResponse> GetActiveProjects(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetActiveProjects request received");

                var projects = await _projectService.GetProjectsByStatusAsync("Open");

                var response = new ProjectListResponse();
                response.Projects.AddRange(projects.Select(project => new ProjectResponse
                {
                    ProjectId = project.Id.ToString(),
                    Title = project.Title,
                    Description = project.Description,
                    Budget = (double)project.Budget,
                    Status = project.Status,
                    ClientId = project.ClientId.ToString()
                }));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetActiveProjects gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}