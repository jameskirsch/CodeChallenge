#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation 
    {
        public Guid CompensationId { get; set; }

        [Required]
        [MaxLength(36)] // match guid restriction // will ensure this is provided later
        public string EmployeeId { get; set; } = null!;

        public Employee? Employee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal? Salary { get; set; }

        public DateTimeOffset? EffectiveDate { get; set; }
    }
}
