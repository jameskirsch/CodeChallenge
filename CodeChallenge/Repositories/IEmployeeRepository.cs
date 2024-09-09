﻿using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetById(string id);
        Task<Employee> GetByIdWithDirectReports(string id);
        Task<Employee> AddAsync(Employee employee);
        Task<Compensation> AddAsync(Compensation compensation);
        Task<Compensation> GetCompensationByEmployeeId(string employeeId);
        Employee Remove(Employee employee);
        Task SaveAsync();
    }
}