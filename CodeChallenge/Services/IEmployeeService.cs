using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Task<Employee> GetById(string id);
        Task<Employee> GetByIdWithDirectReports(string id);
        Task<Compensation> GetCompensationByEmployeeId(string id);
        Task<Employee> Create(Employee employee);
        Task<Compensation> Create(Compensation compensation);
        Task<Employee> Update(Employee existingModel, Employee updateModel);
    }
}
