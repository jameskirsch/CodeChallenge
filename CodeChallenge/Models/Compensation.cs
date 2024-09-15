#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public Guid CompensationId { get; set; }
        public Guid EmployeeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal? Salary { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
    }
}
