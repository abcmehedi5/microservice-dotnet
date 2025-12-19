using JobPortalService.Application.DTOs;
using JobPortalService.Core.Domain.Entities;

namespace JobPortalService.Core.Interfaces
{
    public interface IJobService
    {
        Task<Job> GetJobByIdAsync(Guid jobId);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<IEnumerable<Job>> GetJobsByCompanyAsync(Guid companyId);
        Task<Job> CreateJobAsync(CreateJobDto jobDto);
        Task<Job> UpdateJobAsync(Guid jobId, UpdateJobDto jobDto);
        Task<bool> DeleteJobAsync(Guid jobId);
        Task<Applications> ApplyForJobAsync(Guid jobId, CreateApplicationDto applicationDto);
        Task<IEnumerable<Applications>> GetJobApplicationsAsync(Guid jobId);
    }
}