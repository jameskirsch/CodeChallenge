using System;
using System.Linq;
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
            employee.EmployeeId ??= Guid.NewGuid().ToString();
            await _employeeContext.Employees.AddAsync(employee);
            
            return employee;
        }

        public async Task<Compensation> AddAsync(Compensation compensation)
        {
            if (compensation == null) return null;
            if (compensation.Employee?.EmployeeId == null)
            {
                await AddAsync(compensation.Employee);
            }

            var existing = await GetCompensationByEmployeeId(compensation.Employee?.EmployeeId);
            if (existing != null)
            {
                throw new InvalidOperationException("Compensation for this employee already exists.");
            }
            
            await _employeeContext.Compensation.AddAsync(compensation);

            return compensation;
        }

        public async Task<Compensation> GetCompensationByEmployeeId(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId)) return null;

            var result =
                await _employeeContext.Compensation.SingleOrDefaultAsync(x => x.Employee.EmployeeId == employeeId);

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
                LoadEmployeeDirectReportsWithDfs(employee);
            }

            return employee;
        }

        private async void LoadEmployeeDirectReportsWithDfs(Employee employee)
        {
            _logger.LogDebug("Loading all Direct Reports from Entry Employee");

            await _employeeContext.Entry(employee).Collection(e => e.DirectReports).LoadAsync();

            foreach (var report in employee.DirectReports)
            {
                LoadEmployeeDirectReportsWithDfs(report);
            }
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
