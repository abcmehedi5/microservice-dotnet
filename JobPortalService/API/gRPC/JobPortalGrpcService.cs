using Grpc.Core;
using JobPortalService.Application.DTOs;
using JobPortalService.Core.Interfaces;
using SharedKernel.gRPC;

namespace JobPortalService.API.gRPC
{
    public class JobPortalGrpcService(IJobService jobService, ILogger<JobPortalGrpcService> logger) : JobPortalGrpc.JobPortalGrpcBase
    {
        private readonly IJobService _jobService = jobService;
        private readonly ILogger<JobPortalGrpcService> _logger = logger;

        public override async Task<JobResponse> GetJob(JobRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"gRPC request received for job: {request.JobId}");

            try
            {
                // Parse the job ID
                if (!Guid.TryParse(request.JobId, out Guid jobId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid job ID format"));
                }

                // Get job from service
                var job = await _jobService.GetJobByIdAsync(jobId);

                // Map to gRPC response
                return new JobResponse
                {
                    JobId = job.Id.ToString(),
                    Title = job.Title,
                    Description = job.Description,
                    Status = job.Status,
                    CreatedAt = job.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Job not found: {request.JobId}");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetJob gRPC call: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<JobResponse> CreateJob(CreateJobRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"gRPC request to create job: {request.Title}");

            try
            {
                // Parse company ID
                if (!Guid.TryParse(request.CompanyId, out Guid companyId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid company ID format"));
                }

                // Create DTO from gRPC request
                var jobDto = new CreateJobDto
                {
                    Title = request.Title,
                    Description = request.Description,
                    CompanyId = companyId
                };

                // Call service
                var job = await _jobService.CreateJobAsync(jobDto);

                // Map to gRPC response
                return new JobResponse
                {
                    JobId = job.Id.ToString(),
                    Title = job.Title,
                    Description = job.Description,
                    Status = job.Status,
                    CreatedAt = job.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Company not found: {request.CompanyId}");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in CreateJob gRPC call: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}