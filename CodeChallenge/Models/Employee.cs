#nullable enable
using System;
using System.Collections.Generic;

namespace CodeChallenge.Models
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }

        // Nullable foreign key to reference the parent (or manager)
        public Guid? ParentId { get; set; }  // Parent can be null if the employee is the root

        public List<Employee>? DirectReports { get; set; }
    }
}
