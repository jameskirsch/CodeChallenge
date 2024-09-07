using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        
        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<Employee> Create(Employee employee)
        {
            if (employee == null) return null;
            
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveAsync();

            return employee;
        }

        public async Task<Employee> GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return await _employeeRepository.GetById(id);
            }

            return null;
        }

        public async Task<Compensation> Create(Compensation compensation)
        {
            if (compensation == null) return null;

            // Need to ensure the employee exists before adding a compensation record
            var existingEmployee = await _employeeRepository.GetById(compensation.EmployeeId);

            if (existingEmployee == null) return compensation;

            await _employeeRepository.AddAsync(compensation);
            await _employeeRepository.SaveAsync();

            return compensation;
        }

        public async Task<Compensation> GetCompensationByEmployeeId(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _employeeRepository.GetCompensationByEmployeeId(id);
        }

        public async Task<Employee> GetByIdWithDirectReports(string id)
        {
            return !string.IsNullOrEmpty(id) 
                ? await _employeeRepository.GetByIdWithDirectReports(id) : null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.AddAsync(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
    }
}
