using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public Guid CompensationId { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        public Employee Employee { get; set; }
        
        public decimal Salary { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
    }
}
