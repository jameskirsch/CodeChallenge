using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
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

        public async Task<Compensation> AddAsync(Compensation compensation)
        {
            if (compensation == null) return null;
            if (compensation.Employee?.EmployeeId == null)
            {
                await AddAsync(compensation.Employee);
            }

            var existing = await GetCompensationByEmployeeId(compensation.Employee?.EmployeeId.ToString());
            if (existing != null)
            {
                throw new InvalidOperationException("Compensation for this employee already exists.");
            }
            
            await _employeeContext.Compensations.AddAsync(compensation);
            await _employeeContext.SaveChangesAsync();

            return compensation;
        }

        public async Task<Compensation> GetCompensationByEmployeeId(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId)) return null;

            var result =
                await _employeeContext.Compensations.SingleOrDefaultAsync(x => x.Employee.EmployeeId.ToString() == employeeId);

            return result;
        }

        public async Task<Employee> GetById(string id)
        {
            var result = await _employeeContext.Employees
                .SingleOrDefaultAsync(e => e.EmployeeId.ToString() == id);
            
            return result;
        }
        
        public async Task<Employee> GetByIdWithDirectReports(string id)
        {
            var employee = await _employeeContext.Employees
                .SingleOrDefaultAsync(e => e.EmployeeId.ToString() == id);

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

            foreach (var report in employee.DirectReports)
            {
                await LoadEmployeeDirectReportsWithDfs(report);
            }
        }

        public async Task<Employee> Delete(Employee employee)
        {
            var result = _employeeContext.Remove(employee).Entity;
            await _employeeContext.SaveChangesAsync();
            return result;
        }
    }
}
