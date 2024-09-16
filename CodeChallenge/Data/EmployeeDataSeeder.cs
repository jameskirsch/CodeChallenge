using CodeChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data;

public class EmployeeDataSeeder
{
    private readonly EmployeeContext _employeeContext;
    private const string EmployeeSeedDataFile = "resources/EmployeeSeedData.json";

    public EmployeeDataSeeder(EmployeeContext employeeContext)
    {
        _employeeContext = employeeContext ?? throw new ArgumentNullException(nameof(employeeContext));
    }

    public async Task Seed()
    {
        if(!_employeeContext.Employees.Any())
        {
            var employees = LoadEmployees();
            _employeeContext.Employees.AddRange(employees);
            await _employeeContext.SaveChangesAsync();
        }
    }

    private static List<Employee> LoadEmployees()
    {
        using var fs = new FileStream(EmployeeSeedDataFile, FileMode.Open);
        using var sr = new StreamReader(fs);
        using JsonReader jr = new JsonTextReader(sr);
        var serializer = new JsonSerializer();

        var employees = serializer.Deserialize<List<Employee>>(jr);
        FixUpReferences(employees);

        return employees;
    }

    private static void FixUpReferences(List<Employee> employees)
    {
        var employeeIdRefMap = from employee in employees
            select new { Id = employee.EmployeeId, EmployeeRef = employee };

        employees.ForEach(employee =>
        {
            if (employee.DirectReports == null) return;
            var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
            employee.DirectReports.ForEach(report =>
            {
                var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                referencedEmployees.Add(referencedEmployee);
            });
            employee.DirectReports = referencedEmployees;
        });
    }
}