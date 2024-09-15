using CodeChallenge.Models;
using System;
using System.Threading.Tasks;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _compensationRepository =
                compensationRepository ?? throw new ArgumentNullException(nameof(compensationRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

            if (compensation.EmployeeId == Guid.Empty)
            {
                _logger.LogError("Invalid EmployeeId format.");
                throw new ArgumentException("EmployeeId must be a valid GUID in Compensation Object.", nameof(compensation));
            }

            var existingEmployee = await _employeeRepository.GetById(compensation.EmployeeId);
            if (existingEmployee == null)
            {
                _logger.LogError("No Employee Record found to associate new Compensation Record with");
                throw new InvalidOperationException("Employee not found. Compensation cannot be created.");
            }

            // If the employee exists, attach it to the compensation record.
            compensation.EmployeeId = existingEmployee.EmployeeId;


            // Add the new Compensation Record with the Employee attached, and Save
            await _compensationRepository.AddAsync(compensation);

            return compensation;
        }

        public async Task<Compensation> GetCompensationByEmployeeId(Guid id)
        {
            if (id == Guid.Empty) return null;
            return await _compensationRepository.GetCompensationByEmployeeId(id);
        }
    }
}
