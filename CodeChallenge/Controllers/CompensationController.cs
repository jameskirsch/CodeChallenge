using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CodeChallenge.Models;

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

        [HttpGet("{id}", Name = "getCompensationByEmployeeById")]
        public IActionResult GetCompensationByEmployeeById(string id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _employeeService.GetCompensationByEmployeeId(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        /// <summary>
        /// Create a new Compensation with the Required Employee 
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCompensation([FromBody] Compensation compensation)
        {
            try
            {
                await _employeeService.Create(compensation);
                return CreatedAtRoute("getCompensationByEmployeeById", new { id = compensation.CompensationId}, compensation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating compensation");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
