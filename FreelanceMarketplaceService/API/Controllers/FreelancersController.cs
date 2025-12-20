using AutoMapper;
using FreelanceMarketplaceService.Application.DTOs;
using FreelanceMarketplaceService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplaceService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreelancersController : ControllerBase
    {
        private readonly IFreelancerService _freelancerService;
        private readonly IMapper _mapper;
        private readonly ILogger<FreelancersController> _logger;

        public FreelancersController(IFreelancerService freelancerService, IMapper mapper, ILogger<FreelancersController> logger)
        {
            _freelancerService = freelancerService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SearchFreelancers(
            [FromQuery] string? skill,
            [FromQuery] string? country,
            [FromQuery] decimal? maxHourlyRate)
        {
            try
            {
                var freelancers = await _freelancerService.SearchFreelancersAsync(skill, country, maxHourlyRate);
                var result = _mapper.Map<IEnumerable<FreelancerResponseDto>>(freelancers);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching freelancers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFreelancerById(Guid id)
        {
            try
            {
                var freelancer = await _freelancerService.GetFreelancerByIdAsync(id);
                var result = _mapper.Map<FreelancerResponseDto>(freelancer);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Freelancer not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting freelancer: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFreelancerByUserId(string userId)
        {
            try
            {
                var freelancer = await _freelancerService.GetFreelancerByUserIdAsync(userId);
                var result = _mapper.Map<FreelancerResponseDto>(freelancer);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Freelancer not found for user: {userId}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting freelancer for user: {userId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFreelancer([FromBody] CreateFreelancerDto freelancerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var freelancer = await _freelancerService.CreateFreelancerAsync(freelancerDto);
                var result = _mapper.Map<FreelancerResponseDto>(freelancer);
                return CreatedAtAction(nameof(GetFreelancerById), new { id = freelancer.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Duplicate freelancer profile for user: {freelancerDto.UserId}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating freelancer");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFreelancer(Guid id, [FromBody] UpdateFreelancerDto freelancerDto)
        {
            try
            {
                var freelancer = await _freelancerService.UpdateFreelancerAsync(id, freelancerDto);
                var result = _mapper.Map<FreelancerResponseDto>(freelancer);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Freelancer not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating freelancer: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/portfolio")]
        public async Task<IActionResult> AddPortfolioItem(Guid id, [FromBody] CreatePortfolioItemDto portfolioDto)
        {
            try
            {
                var portfolioItem = await _freelancerService.AddPortfolioItemAsync(id, portfolioDto);
                return Ok(new
                {
                    message = "Portfolio item added successfully",
                    portfolioItemId = portfolioItem.Id
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Freelancer not found: {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding portfolio item for freelancer: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/portfolio")]
        public async Task<IActionResult> GetFreelancerPortfolio(Guid id)
        {
            try
            {
                var portfolio = await _freelancerService.GetFreelancerPortfolioAsync(id);
                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting portfolio for freelancer: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}