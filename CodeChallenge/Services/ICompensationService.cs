using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Task<Compensation> GetCompensationByEmployeeId(Guid id);
        Task<Compensation> Create(Compensation compensation);
    }
}