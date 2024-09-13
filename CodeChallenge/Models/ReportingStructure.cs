using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        [JsonPropertyName("Employee")]
        public Employee Employee { get; set; }

        // should equal the total number of reports under a given employee
        // is determined to be the number of directReports for an employee and all of their direct reports
        public int? NumberOfReports { get; set; }
    }
}
