using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services;

public class ReportingStructureService : IReportingStructureService
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<IReportingStructureService> _logger;

    // want to de-couple with DI (Dependency Injection)
    public ReportingStructureService(
        ILogger<ReportingStructureService> logger, 
        IEmployeeService employeeService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
    }

    /// <summary>
    /// Returns the Reporting Structure with the total Number of Reports by EmployeeId
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ReportingStructure> GetReportingStructureByEmployeeId(Guid id)
    {
        _logger.LogDebug("Getting Reporting Structure By EmployeeId");

        var employee = await _employeeService.GetById(id);
            
        if (employee == null) return null;

        var reportingStructure = new ReportingStructure
        {
            Employee = employee,
            NumberOfReports = await GetReportCount(employee)
        };

        return reportingStructure;
    }

    /// <summary>
    /// This method employs an iterative Depth-First Search (DFS) algorithm to traverse the employee hierarchy
    /// and calculate the total number of direct and indirect reports. The iterative DFS approach is chosen for 
    /// its scalability and its ability to avoid stack overflow issues that can arise with recursive implementations 
    /// in deeply nested hierarchies.
    ///
    /// DFS provides a time complexity of O(n), where n represents the total number of employees in the hierarchy, 
    /// making it an efficient solution for large datasets. Additionally, the space complexity is O(d), where d is 
    /// the maximum depth of the hierarchy. This space efficiency is crucial for large organizations, as it ensures 
    /// memory usage remains proportional to the depth of the hierarchy rather than the total number of employees.
    ///
    /// In real-world cases like Walmart, with over 2.1 million employees, efficient traversal of such hierarchies 
    /// is essential. While DFS works well for deep structures, a Breadth-First Search (BFS) could be more suitable 
    /// for wider hierarchies, where the number of employees per level is large. Choosing the right algorithm should 
    /// consider the shape of the hierarchy being processed.
    ///
    /// To prevent the N+1 query problem, this implementation explicitly loads the next set of direct reports. 
    /// Although Lazy Loading is enabled, Eagerly Loading the reports reduces the number of database round trips, 
    /// optimizing performance when fetching hierarchical data.
    ///
    /// In practice, applying restrictions on hierarchy depth can prevent excessive memory consumption and query 
    /// overhead. Additionally, techniques like Lazy or Eager loading can be adjusted to further improve performance.
    ///
    /// A potential alternative considered was Recursive Projection, which would allow querying the entire hierarchy 
    /// in a single operation at the database level. However, in this case, the iterative DFS approach is better suited 
    /// for managing memory efficiently with larger data sets.
    ///
    /// Please note that any adjustments to the original code reflect my approach to solving similar challenges in a 
    /// real-world environment. Thank you for reviewing my solution.
    /// </summary>
    /// <param name="employee">The root employee from which to begin the hierarchy traversal.</param>
    /// <returns>The total number of direct and indirect reports for the given employee.</returns>
    public async Task<int?> GetReportCount(Employee employee)
    {
        _logger.LogDebug("Getting Report Count by Employee Hierarchy");

        var totalReports = 0;
        var stack = new Stack<Employee>();

        // Add the root employee to the stack to begin the traversal
        stack.Push(employee);

        // Iterate through the employee hierarchy using Depth-First Search (DFS)
        while (stack.Count > 0)
        {
            // Remove and temporarily store the next employee from the stack
            var currentEmployee = stack.Pop();
           
            // Load the next set of Direct Reports with Eager Loading to avoid the N+1 Query Problem.
            await _employeeService.SetEmployeeDirectReports(currentEmployee);

            // If the current employee has direct reports, process them
            if (currentEmployee.DirectReports == null) continue;
            
            totalReports += currentEmployee.DirectReports.Count;

            // Push the Reports onto the Stack
            foreach (var directReport in currentEmployee.DirectReports)
            {
                stack.Push(directReport);
            }
        }

        return totalReports;
    }
}