using System;
using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Task<Employee> GetById(Guid id);
        Task<Employee> GetByIdWithDirectReports(Guid id);
        Task<Employee> Create(Employee employee);
        Task<Employee> Update(Employee existingModel, Employee updateModel);
    }
}
