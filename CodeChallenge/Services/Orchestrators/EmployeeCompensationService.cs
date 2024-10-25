using CodeChallenge.Data;
using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services.Orchestrators
{
    public class EmployeeCompensationService : IEmployeeCompensationService
    {
        private readonly EmployeeService _employeeService;
        private readonly CompensationService _compensationService;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeCompensationService(
            EmployeeService employeeService,
            CompensationService compensationService,
            IUnitOfWork unitOfWork)
        {
            _employeeService = employeeService;
            _compensationService = compensationService;
            _unitOfWork = unitOfWork;
        }

        public async Task AddEmployeeWithCompensationAsync(Employee employee, Compensation compensation)
        {
            await _employeeService.AddAsync(employee, true);
            await _compensationService.AddAsync(compensation, true);

            // Ensures that both changes are saved together
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddCompensationToExistingEmployee()
        {
            var employee = await _employeeService.GetAllEmployeesAsync();
        }
    }
}
