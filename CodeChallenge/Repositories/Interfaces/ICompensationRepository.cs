using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories.Interfaces;

public interface ICompensationRepository : IRepository<Compensation>
{
    Task<Compensation> GetCompensationByEmployeeId(Guid employeeId);
}