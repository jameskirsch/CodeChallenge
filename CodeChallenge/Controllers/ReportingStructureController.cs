using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/reporting")]
    public class ReportingStructureController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IReportingStructureService _reportingStructureService;

        public ReportingStructureController(
            ILogger<EmployeeController> logger,
            IReportingStructureService reportingStructureService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _reportingStructureService =
                reportingStructureService ?? throw new ArgumentNullException(nameof(reportingStructureService));
        }

        [HttpGet("{id}", Name = "getReportStructureByEmployeeId")]
        public async Task<IActionResult> GetReportStructureByEmployeeId(String id)
        {
            try
            {
                _logger.LogDebug("Requesting Reporting Structure Based on Employee Id");

                var reportingStructure = await _reportingStructureService.GetReportingStructureByEmployeeId(id);
                if (reportingStructure == null)
                    return NotFound();

                return Ok(reportingStructure);

            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
