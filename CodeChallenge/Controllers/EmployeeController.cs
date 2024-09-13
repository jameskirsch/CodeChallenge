using System;
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
            _logger.LogDebug("Received employee create request for '{FirstName} {LastName}'", employee.FirstName,
                employee.LastName);
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
        public async Task<IActionResult> Update(string id, [FromBody]Employee updateModel)
        {
            _logger.LogDebug("Received employee update request for '{id}'", id);

            var existingEmployee = await _employeeService.GetById(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            var updatedEmployee = await _employeeService.Update(existingEmployee, updateModel);

            return Ok(updatedEmployee);
        }

        //TODO: Setup Delete Operation for Employee
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            throw new NotImplementedException("Still need to setup");
        }
    }
}
