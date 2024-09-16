using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using CodeChallenge.ViewModels;

namespace CodeChallenge.Controllers;

[ApiController]
[Route("api/employee")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)) ;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        _logger.LogDebug("Received employee create request for '{FirstName} {LastName}'", employee.FirstName,
            employee.LastName);
        await _employeeService.Create(employee);

        return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
    }

    [HttpGet("{id:guid}", Name = "getEmployeeById")]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        _logger.LogDebug("Received employee get request for '{id}'", id);

        var employee = await _employeeService.GetById(id);

        if (employee == null)
            return NotFound();

        var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);

        return Ok(employeeViewModel);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody]Employee updateModel)
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
}