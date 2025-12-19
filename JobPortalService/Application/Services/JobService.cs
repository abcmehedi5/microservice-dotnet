using AutoMapper;
using JobPortalService.Application.DTOs;
using JobPortalService.Core.Domain.Entities;
using JobPortalService.Core.Interfaces;
using JobPortalService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPortalService.Application.Services
{
    public class JobService(JobPortalDbContext context, IMapper mapper, ILogger<JobService> logger) : IJobService
    {
        private readonly JobPortalDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<JobService> _logger = logger;

        public async Task<Job> GetJobByIdAsync(Guid jobId)
        {
            _logger.LogInformation($"Getting job with ID: {jobId}");

            var job = await _context.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found");
            }

            return job;
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            _logger.LogInformation("Getting all jobs");

            return await _context.Jobs
                .Include(j => j.Company)
                .Where(j => j.Status == "Active")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByCompanyAsync(Guid companyId)
        {
            _logger.LogInformation($"Getting jobs for company ID: {companyId}");

            return await _context.Jobs
                .Include(j => j.Company)
                .Where(j => j.CompanyId == companyId && j.Status == "Active")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Job> CreateJobAsync(CreateJobDto jobDto)
        {
            _logger.LogInformation($"Creating new job: {jobDto.Title}");

            // Check if company exists
            var company = await _context.Companies.FindAsync(jobDto.CompanyId);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {jobDto.CompanyId} not found");
            }

            var job = new Job
            {
                Title = jobDto.Title,
                Description = jobDto.Description,
                Salary = jobDto.Salary,
                Location = jobDto.Location,
                JobType = jobDto.JobType,
                CompanyId = jobDto.CompanyId,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<Job> UpdateJobAsync(Guid jobId, UpdateJobDto jobDto)
        {
            _logger.LogInformation($"Updating job with ID: {jobId}");

            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found");
            }

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(jobDto.Title))
                job.Title = jobDto.Title;

            if (!string.IsNullOrWhiteSpace(jobDto.Description))
                job.Description = jobDto.Description;

            if (jobDto.Salary.HasValue)
                job.Salary = jobDto.Salary.Value;

            if (!string.IsNullOrWhiteSpace(jobDto.Location))
                job.Location = jobDto.Location;

            if (!string.IsNullOrWhiteSpace(jobDto.JobType))
                job.JobType = jobDto.JobType;

            if (!string.IsNullOrWhiteSpace(jobDto.Status))
                job.Status = jobDto.Status;

            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<bool> DeleteJobAsync(Guid jobId)
        {
            _logger.LogInformation($"Deleting job with ID: {jobId}");

            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found");
            }

            // Soft delete by changing status
            job.Status = "Deleted";
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Applications> ApplyForJobAsync(Guid jobId, CreateApplicationDto applicationDto)
        {
            _logger.LogInformation($"Applying for job ID: {jobId} by applicant: {applicationDto.ApplicantId}");

            // Check if job exists and is active
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.Status != "Active")
            {
                throw new KeyNotFoundException($"Active job with ID {jobId} not found");
            }

            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicationDto.ApplicantId);

            if (existingApplication != null)
            {
                throw new InvalidOperationException($"Applicant {applicationDto.ApplicantId} has already applied for job {jobId}");
            }

            var application = new Applications
            {
                JobId = jobId,
                ApplicantId = applicationDto.ApplicantId,
                CoverLetter = applicationDto.CoverLetter,
                ResumeUrl = applicationDto.ResumeUrl,
                Status = "Applied",
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<IEnumerable<Applications>> GetJobApplicationsAsync(Guid jobId)
        {
            _logger.LogInformation($"Getting applications for job ID: {jobId}");

            return await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }
    }
}