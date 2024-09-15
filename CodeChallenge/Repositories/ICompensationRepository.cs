using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Task<Compensation> AddAsync(Compensation compensation);
        Task<Compensation> GetCompensationByEmployeeId(Guid employeeId);
    }
}
