using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;
using CodeChallenge.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CodeChallenge.Services
{
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

        public async Task<Employee> Create(Employee employee)
        {
            if (employee == null) return null;

            await _employeeRepository.AddAsync(employee);

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

        /// <summary>
        /// Creates a new Compensation record. Requires at least an EmployeeId to be associated.
        /// Any invalid or missing EmployeeId results in a propagated exception to the controller,
        /// which should handle it and return an appropriate HTTP response.
        /// </summary>
        /// <param name="compensation">The compensation object to be created.</param>
        /// <returns>The created compensation object, or an exception if validation fails.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the compensation object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the EmployeeId is missing or null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the associated Employee does not exist.</exception>
        public async Task<Compensation> Create(Compensation compensation)
        {
            // Ensure compensation exists
            if (compensation == null)
            {
                _logger.LogError("Compensation Object is Null");
                throw new ArgumentNullException(nameof(compensation), "Compensation objection cannot be null");
            }

            //// Check if EmployeeId is a valid GUID
            if (string.IsNullOrEmpty(compensation.EmployeeId) || !Guid.TryParse(compensation.EmployeeId, out var employeeGuid))
            {
                _logger.LogError("Invalid EmployeeId format.");
                throw new ArgumentException("EmployeeId must be a valid GUID in Compensation Object.", nameof(compensation));
            }

            // Check for existing employee, if not found, return appropriate error
            var existingEmployee = await _employeeRepository.GetById(compensation.EmployeeId);
            if (existingEmployee == null)
            {
                _logger.LogError("No Employee Record found to associate new Compensation Record with");
                throw new InvalidOperationException("Employee not found. Compensation cannot be created.");
            }
            
            // If the employee exists, attach it to the compensation record.
            compensation.Employee = existingEmployee;

            // Add the new Compensation Record with the Employee attached, and Save
            await _employeeRepository.AddAsync(compensation);

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

        public async Task<Employee> Update(Employee existingModel, Employee updateModel)
        {
            if (existingModel == null) return updateModel;
            
            _mapper.Map(updateModel, existingModel);
            var result = await _employeeRepository.Update(existingModel);

            return result;
        }
    }
}
