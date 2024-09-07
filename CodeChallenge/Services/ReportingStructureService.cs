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
        public async Task<ReportingStructure> GetReportingStructureByEmployeeId(string id)
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
        /// Instead of using a direct recursive solution to solve this problem,
        /// I opted to use a (DFS) Depth First Search algorithm
        /// My reasoning behind this, is for scalability, the time complexity is still O(n), but this avoids
        /// memory/ stack overflow issues on deeply nested tree hierarchies
        /// This could be argued to be a (BFS) Breadth - First Search depending on how it scales
        /// I would determine which technique to use based on whether the structure is more prone to being
        /// wide tree vs narrow in depth tree, could possibly add limits to the nesting level in a real situation
        /// then choose which algorithm is best suited and go from there, if the tree is too wide it would have to
        /// hold a lot of memory at once at that level, maybe a discussion point?
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public int GetReportCount(Employee employee)
        {
            _logger.LogDebug("Getting Report Count by Employee Hierarchy");

            if (employee == null) return 0;

            var count = 0;
            var stack = new Stack<Employee>();
            
            stack.Push(employee);

            while (stack.Count > 0)
            {
                var currentEmployee = stack.Pop();

                // Add each direct report to the stack and increment
                if (currentEmployee?.DirectReports == null) continue;
                foreach (var report in currentEmployee?.DirectReports)
                {
                    count++;
                    stack.Push(report);
                }
            }

            return count;
        }
    }
}
