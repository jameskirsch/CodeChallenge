using System;
using System.Threading.Tasks;
using CodeChallenge.Models;

namespace CodeChallenge.Services;

public interface IReportingStructureService
{
    public Task<ReportingStructure> GetReportingStructureByEmployeeId(Guid id);
}