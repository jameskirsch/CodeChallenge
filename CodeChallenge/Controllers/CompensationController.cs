using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CodeChallenge.Models;
using Microsoft.AspNetCore.Http;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves the Compensation record associated with a specific Employee by EmployeeId.
        /// </summary>
        /// <param name="id">The EmployeeId used to look up the corresponding Compensation record.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with the Compensation record if found.
        /// If no Compensation record is found for the provided EmployeeId, returns a 404 Not Found response.
        /// </returns>
        [HttpGet("{id}", Name = "getCompensationByEmployeeById")]
        public async Task<IActionResult> GetCompensationByEmployeeById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("EmployeeId is null or empty in GetCompensationByEmployeeById.");
                    return BadRequest(new { message = "EmployeeId is required." });
                }

                _logger.LogDebug("Received compensation get request for EmployeeId '{EmployeeId}'", id);

                var compensation = await _employeeService.GetCompensationByEmployeeId(id);
                if (compensation == null)
                {
                    _logger.LogInformation("No Compensation record found for EmployeeId '{EmployeeId}'", id);
                    return NotFound();
                }

                _logger.LogDebug("Successfully retrieved compensation record for EmployeeId '{EmployeeId}'", id);
                return Ok(compensation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving compensation for EmployeeId '{EmployeeId}'", id);
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        /// <summary>
        /// Creates a new Compensation record with the associated Employee.
        /// </summary>
        /// <param name="compensation">The Compensation object to be created, which includes the required employee details and compensation information.</param>
        /// <returns>
        /// Returns a 201 Created response if the Compensation record is successfully created.
        /// If an error occurs during creation, returns a 500 Internal Server Error response with a corresponding error message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateCompensation([FromBody] Compensation compensation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid compensation data. EmployeeId is missing.");
                    return BadRequest(new { message = "Invalid data. EmployeeId is required." });
                }

                var createdCompensationResult = await _employeeService.Create(compensation);
                if (createdCompensationResult == null)
                {
                    _logger.LogWarning("Failed to create compensation record for EmployeeId '{EmployeeId}'", compensation.EmployeeId);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create compensation. Please try again later." });
                }

                _logger.LogInformation("Successfully created compensation record for EmployeeId '{EmployeeId}'", compensation.EmployeeId);
                return CreatedAtRoute("getCompensationByEmployeeById", new { id = compensation.CompensationId}, compensation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating compensation for EmployeeId '{EmployeeId}'", compensation?.EmployeeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
