using AutoMapper;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplaceService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, IMapper mapper, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                var result = _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all projects");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                var result = _mapper.Map<ProjectResponseDto>(project);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Project not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting project: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetProjectsByClient(Guid clientId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByClientAsync(clientId);
                var result = _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting projects for client: {clientId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetProjectsByStatus(string status)
        {
            try
            {
                var projects = await _projectService.GetProjectsByStatusAsync(status);
                var result = _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting projects with status: {status}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var project = await _projectService.CreateProjectAsync(projectDto);
                var result = _mapper.Map<ProjectResponseDto>(project);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Client not found: {projectDto.ClientId}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto projectDto)
        {
            try
            {
                var project = await _projectService.UpdateProjectAsync(id, projectDto);
                var result = _mapper.Map<ProjectResponseDto>(project);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Project not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating project: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CloseProject(Guid id)
        {
            try
            {
                await _projectService.CloseProjectAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Project not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error closing project: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/bids")]
        public async Task<IActionResult> SubmitBid(Guid id, [FromBody] CreateBidDto bidDto)
        {
            try
            {
                var bid = await _projectService.SubmitBidAsync(id, bidDto);
                var result = _mapper.Map<BidResponseDto>(bid);
                return Ok(new
                {
                    message = "Bid submitted successfully",
                    bid = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Project or freelancer not found for bid submission");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Duplicate bid submission");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting bid for project: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bids/{bidId}/accept")]
        public async Task<IActionResult> AcceptBid(Guid bidId)
        {
            try
            {
                var bid = await _projectService.AcceptBidAsync(bidId);
                var result = _mapper.Map<BidResponseDto>(bid);
                return Ok(new
                {
                    message = "Bid accepted successfully",
                    bid = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Bid not found: {bidId}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Cannot accept bid: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error accepting bid: {bidId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/bids")]
        public async Task<IActionResult> GetProjectBids(Guid id)
        {
            try
            {
                var bids = await _projectService.GetProjectBidsAsync(id);
                var result = _mapper.Map<IEnumerable<BidResponseDto>>(bids);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bids for project: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}