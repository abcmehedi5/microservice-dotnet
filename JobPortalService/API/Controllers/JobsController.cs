using AutoMapper;
using JobPortalService.Application.DTOs;
using JobPortalService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController(IJobService jobService, IMapper mapper, ILogger<JobsController> logger) : ControllerBase
    {
        private readonly IJobService _jobService = jobService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<JobsController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync();
                var result = _mapper.Map<IEnumerable<JobResponseDto>>(jobs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all jobs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                var result = _mapper.Map<JobResponseDto>(job);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Job not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting job: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto jobDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var job = await _jobService.CreateJobAsync(jobDto);
                var result = _mapper.Map<JobResponseDto>(job);
                return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Company not found: {jobDto.CompanyId}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobDto jobDto)
        {
            try
            {
                var job = await _jobService.UpdateJobAsync(id, jobDto);
                var result = _mapper.Map<JobResponseDto>(job);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Job not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating job: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            try
            {
                await _jobService.DeleteJobAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Job not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting job: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyForJob(Guid id, [FromBody] CreateApplicationDto applicationDto)
        {
            try
            {
                var application = await _jobService.ApplyForJobAsync(id, applicationDto);
                return Ok(new
                {
                    message = "Application submitted successfully",
                    applicationId = application.Id
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Job not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Duplicate application for job: {id}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error applying for job: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetJobApplications(Guid id)
        {
            try
            {
                var applications = await _jobService.GetJobApplicationsAsync(id);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting applications for job: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}