using System;
using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories;

public interface IEmployeeRepository
{
    Task<Employee> GetById(Guid id);
    Task<Employee> GetByIdWithDirectReports(Guid id);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> Update(Employee employee);
}