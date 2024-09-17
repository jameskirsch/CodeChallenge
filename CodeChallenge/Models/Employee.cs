#nullable enable
using System;
using System.Collections.Generic;

namespace CodeChallenge.Models;

public class Employee
{
    public Guid EmployeeId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }

    // Foreign key for parent, null if this is the root employee
    public Guid? ParentId { get; set; }

    // Navigation property to the parent employee
    public virtual Employee? Parent { get; set; }

    // Navigation property for direct reports (Lazy Loading enabled)
    public virtual List<Employee>? DirectReports { get; set; }
}