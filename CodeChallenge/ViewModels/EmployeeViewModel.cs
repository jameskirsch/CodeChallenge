#nullable enable
using System;
using System.Collections.Generic;

namespace CodeChallenge.ViewModels;

public class EmployeeViewModel
{
    public Guid EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public List<EmployeeViewModel>? DirectReports { get; set; }
}