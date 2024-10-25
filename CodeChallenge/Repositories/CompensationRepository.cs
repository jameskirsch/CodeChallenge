﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using CodeChallenge.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Repositories;

public class CompensationRepository : ICompensationRepository
{
    private readonly CompensationContext _compensationContext;
    private readonly EmployeeContext _employeeContext;
    private readonly ILogger<ICompensationRepository> _logger;

    public CompensationRepository(ILogger<CompensationRepository> logger, CompensationContext compensationContext, EmployeeContext employeeContext)
    {
        _compensationContext = compensationContext ?? throw new ArgumentNullException(nameof(compensationContext));
        _employeeContext = employeeContext ?? throw new ArgumentNullException(nameof(employeeContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Compensation> AddAsync(Compensation compensation)
    {
        _logger.LogInformation("Attempting to Create new Compensation Record.");

        if (compensation?.EmployeeId == null)
        {
            throw new InvalidOperationException("EmployeeId is required.");
        }

        if (await _employeeContext.Employees.FindAsync(compensation.EmployeeId) == null)
        {
            throw new InvalidOperationException("Employee does not exist.");
        }

        var existing = await GetCompensationByEmployeeId(compensation.EmployeeId);
        if (existing != null)
        {
            throw new InvalidOperationException("Compensation for this employee already exists.");
        }

        await _compensationContext.Compensations.AddAsync(compensation);

        return compensation;
    }

    public void Delete(Compensation entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Compensation>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Compensation> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Compensation> GetCompensationByEmployeeId(Guid employeeId)
    {
        if (employeeId == Guid.Empty) return null;

        var result = await _compensationContext.Compensations
            .SingleOrDefaultAsync(x => x.EmployeeId == employeeId);

        return result;
    }

    public Task SaveChangesAsync()
    {
        return _compensationContext.SaveChangesAsync();
    }

    public void Update(Compensation entity)
    {
        throw new NotImplementedException();
    }
}