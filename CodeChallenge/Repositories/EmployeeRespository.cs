using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeContext _employeeContext;
    private readonly ILogger<IEmployeeRepository> _logger;

    public EmployeeRepository(ILogger<EmployeeRepository> logger, EmployeeContext employeeContext)
    {
        _employeeContext = employeeContext ?? throw new ArgumentNullException(nameof(employeeContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        if (employee == null) return null;

        await _employeeContext.Employees.AddAsync(employee);
        await _employeeContext.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee> Update(Employee employee)
    {
        if (employee == null || employee.EmployeeId == Guid.Empty) return null;

        var result = _employeeContext.Employees.Update(employee).Entity;
        await _employeeContext.SaveChangesAsync();

        return result;
    }

    public async Task<Employee> GetById(Guid id)
    {
        var result = await _employeeContext.Employees
            .SingleOrDefaultAsync(e => e.EmployeeId == id);

        return result;
    }

    public async Task<Employee> GetByIdWithDirectReports(Guid id)
    {
        var employee = await _employeeContext.Employees
            .SingleOrDefaultAsync(e => e.EmployeeId == id);

        if (employee != null)
        {
            await LoadEmployeeDirectReportsWithDfs(employee);
        }

        return employee;
    }

    private async Task LoadEmployeeDirectReportsWithDfs(Employee employee)
    {
        _logger.LogDebug("Loading all Direct Reports from Entry Employee");
        await _employeeContext.Entry(employee).Collection(e => e.DirectReports).LoadAsync();

        if (employee.DirectReports != null)
        {
            foreach (var report in employee.DirectReports)
            {
                await LoadEmployeeDirectReportsWithDfs(report);
            }
        }
    }
}