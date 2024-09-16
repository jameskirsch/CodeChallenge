using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data;

public class EmployeeContext : DbContext
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define Employee entity with a self-referencing relationship for DirectReports
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.EmployeeId);

        // Configure self-referencing relationship: Employee has many DirectReports and one Parent
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.DirectReports)
            .WithOne()  // No navigation property for Parent (handled only through ParentId)
            .HasForeignKey(e => e.ParentId)  // Foreign key is explicitly set to ParentId (optional)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes

        // Define ReportingStructure entity and relationship with Employee
        modelBuilder.Entity<ReportingStructure>()
            .HasOne(rs => rs.Employee)
            .WithMany()
            .IsRequired();
    }
}