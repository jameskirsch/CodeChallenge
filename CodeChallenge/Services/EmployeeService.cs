using System;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using AutoMapper;

namespace CodeChallenge.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IMapper _mapper;
        
    public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Employee> Create(Employee employee)
    {
        if (employee == null) return null;

        await _employeeRepository.AddAsync(employee);

        return employee;
    }

    public async Task<Employee> GetById(Guid id)
    {
        if(id != Guid.Empty)
        {
            return await _employeeRepository.GetById(id);
        }

        return null;
    }

    public async Task<Employee> GetByIdWithDirectReports(Guid id)
    {
        return id != Guid.Empty 
            ? await _employeeRepository.GetByIdWithDirectReports(id) : null;
    }

    public async Task<Employee> Update(Employee existingModel, Employee updateModel)
    {
        if (existingModel == null) return updateModel;
            
        _mapper.Map(updateModel, existingModel);
        var result = await _employeeRepository.Update(existingModel);

        return result;
    }
}