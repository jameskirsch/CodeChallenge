using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using CodeChallenge.Repositories.Interfaces;

namespace CodeChallenge.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IMapper _mapper;

    public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Employee> GetById(Guid id)
    {
        if (id != Guid.Empty)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        return null;
    }

    public async Task SetEmployeeDirectReports(Employee employee)
    {
        await _employeeRepository.SetEmployeeDirectReportCollection(employee);
    }

    public void Update(Employee existingEmployee, Employee UpdatedEmployee)
    {
        _mapper.Map(UpdatedEmployee, existingEmployee);
        _employeeRepository.Update(existingEmployee);
    }

    public async Task<Employee> AddAsync(Employee employee, bool? deferCommitToUoW = false)
    {
        _logger.LogInformation("Attempting to Create a new Employee Record");

        if (employee == null) return null;

            var result = await _employeeRepository.AddAsync(employee);

        if (!(deferCommitToUoW ?? true))
        {
            await _employeeRepository.SaveChangesAsync();
        }

        return result;
    }

    public async Task<Employee> GetEmployeeByIdAsync(Guid id)
    {
        return await _employeeRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _employeeRepository.GetAllAsync();
    }

    public async Task<Employee> Update(Employee UpdatedEmployee, bool? deferCommitToUoW = false)
    {
        var existingEmployee = await _employeeRepository.GetByIdAsync(UpdatedEmployee.EmployeeId);
        if (existingEmployee == null) return null;
        
        var mappedEntity = _mapper.Map(UpdatedEmployee, existingEmployee);
        _employeeRepository.Update(mappedEntity);

        if (!(deferCommitToUoW ?? true))
        {
            await _employeeRepository.SaveChangesAsync();
        }

        return existingEmployee;
    }

    public void DeleteEmployee(Employee employee)
    {
        _employeeRepository.Delete(employee);
        _employeeRepository.SaveChangesAsync();
    }
}