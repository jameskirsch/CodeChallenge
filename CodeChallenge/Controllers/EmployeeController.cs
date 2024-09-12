using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug("Received employee create request for '{FirstName} {LastName}'", employee.FirstName, employee.LastName);
            await _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            _logger.LogDebug("Received employee get request for '{id}'", id);

            var employee = await _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ReplaceEmployee(string id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug("Received employee update request for '{id}'", id);
            
            var existingEmployee = await _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
    }
}
