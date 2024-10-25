using System;
using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task SetEmployeeDirectReportCollection(Employee employee);
}