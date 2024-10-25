using System;
using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services;

public interface IEmployeeService
{
    Task<Employee> GetById(Guid id);
    Task<Employee> Update(Employee UpdatedEmployee, bool? deferCommitToUoW = false);
    Task SetEmployeeDirectReports(Employee employee);
    Task<Employee> AddAsync(Employee employee, bool? deferCommitToUoW = false);
}