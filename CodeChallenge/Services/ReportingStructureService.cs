using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services
{
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

            var employee = await _employeeService.GetByIdWithDirectReports(id);
            
            if (employee == null) return null;

            var reportingStructure = new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = GetReportCount(employee)
            };

            return reportingStructure;
        }

        /// <summary>
        /// This method uses an iterative Depth-First Search (DFS) algorithm to traverse the employee hierarchy 
        /// and calculate the total number of direct and indirect reports. The DFS approach is chosen for its scalability 
        /// and to avoid potential stack overflow issues that could occur with a recursive implementation in deeply nested hierarchies. 
        /// 
        /// In large organizations, such as Walmart with over 2.1 million employees, efficient traversal of hierarchical structures 
        /// is crucial. The DFS algorithm ensures that we maintain O(n) time complexity, where n is the total number of employees in the hierarchy, 
        /// but more importantly, it manages space complexity at O(d), where d represents the maximum depth of the tree. This is critical 
        /// in scenarios with deeply nested hierarchies, as it ensures that memory consumption is proportional to the depth, rather than the total size, 
        /// allowing the algorithm to efficiently handle even large, deeply nested structures without excessive memory use.
        /// 
        /// While DFS is well-suited for deeply nested structures, a Breadth-First Search (BFS) approach may be more appropriate 
        /// in wide hierarchies, where memory consumption at each level can grow significantly. In real-world applications, analyzing the 
        /// data structure (whether it's deeper or wider) can help determine the best algorithm for traversal.
        ///
        /// Maybe a discussion point? (I think also in a real scenario it might be viable to put restrictions on how deep the nesting can be)
        /// Some of the overhead can be managed with lazy loading/eager loading as well I think.
        ///
        /// Also, I just want to say that anything I've changed was not a critique on any of the code given
        /// but just trying to display how I might program something on the Job, and I want to thank you
        /// for taking the time to review my solution, and I am happy to have had a chance to show some of my work.
        /// </summary>
        /// <param name="employee">The root employee from which to start the hierarchy traversal.</param>
        /// <returns>The total number of direct and indirect reports for the given employee.</returns>
        public int GetReportCount(Employee employee)
        {
            _logger.LogDebug("Getting Report Count by Employee Hierarchy");

            if (employee == null) return 0;

            var count = 0;
            var stack = new Stack<Employee>();

            // Add the root employee to the stack to begin the traversal
            stack.Push(employee);

            // Iterate through the employee hierarchy using Depth-First Search (DFS)
            while (stack.Count > 0)
            {
                // Remove and temporarily store the next employee from the stack
                var currentEmployee = stack.Pop();

                // If the current employee has direct reports, process them
                if (currentEmployee?.DirectReports == null) continue;
                foreach (var directReport in currentEmployee.DirectReports)
                {
                    count++;
                    stack.Push(directReport);
                }
            }

            return count;
        }
    }
}
