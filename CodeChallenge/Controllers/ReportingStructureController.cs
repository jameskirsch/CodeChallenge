using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using CodeChallenge.ViewModels;

namespace CodeChallenge.Controllers;

[ApiController]
[Route("api/reporting")]
public class ReportingStructureController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IReportingStructureService _reportingStructureService;
    private readonly IMapper _mapper;

    public ReportingStructureController(
        ILogger<EmployeeController> logger,
        IReportingStructureService reportingStructureService, 
        IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _reportingStructureService =
            reportingStructureService ?? throw new ArgumentNullException(nameof(reportingStructureService));
    }

    /// <summary>
    /// Retrieves the reporting structure for an employee based on the provided EmployeeId.
    /// </summary>
    /// <param name="id">The EmployeeId used to retrieve the reporting structure.</param>
    /// <returns>
    /// Returns a 200 OK response with the reporting structure if found.
    /// Returns a 404 Not Found response if no reporting structure is found for the given EmployeeId.
    /// Returns a 500 Internal Server Error response if an unexpected error occurs.
    /// </returns>
    [HttpGet("{id:guid}", Name = "getReportStructureByEmployeeId")]
    public async Task<IActionResult> GetReportStructureByEmployeeId(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid EmployeeId: EmployeeId is an invalid Guid.");
                return BadRequest(new { message = "EmployeeId is required." });
            }

            _logger.LogDebug("Requesting Reporting Structure for EmployeeId: {EmployeeId}", id);

            var reportingStructure = await _reportingStructureService.GetReportingStructureByEmployeeId(id);
                
            if (reportingStructure == null)
            {
                _logger.LogInformation("No Reporting Structure found for EmployeeId {EmployeeId}", id);
                return NotFound(new { message = $"No reporting structure found for EmployeeId {id}." });
            }

            var reportingStructureViewModel = _mapper.Map<ReportingStructureViewModel>(reportingStructure);

            _logger.LogDebug("Successfully retrieved Reporting Structure for EmployeeId: {EmployeeId}", id);
            return Ok(reportingStructureViewModel);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the reporting structure for EmployeeId: {EmployeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}