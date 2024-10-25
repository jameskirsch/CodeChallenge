using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeCompensationService
    {
        Task AddEmployeeWithCompensationAsync(Employee employee, Compensation compensation);
    }
}
