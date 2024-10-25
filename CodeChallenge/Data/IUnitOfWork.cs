using CodeChallenge.Models;
using CodeChallenge.Repositories.Interfaces;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public interface IUnitOfWork
    {
        IRepository<Employee> Employees { get; }
        IRepository<Compensation> Compensations { get; }
        Task SaveChangesAsync();
    }
}
