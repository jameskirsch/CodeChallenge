using System;

namespace CodeChallenge.Models;

public class ReportingStructure
{
    public Guid ReportingStructureId { get; set; }
    public Employee Employee { get; set; }

    // Represents the total count of all direct and indirect reports under a given employee.
    // This includes the employee's immediate direct reports as well as all subsequent levels of reports in the hierarchy.
    public int? NumberOfReports { get; set; }
}