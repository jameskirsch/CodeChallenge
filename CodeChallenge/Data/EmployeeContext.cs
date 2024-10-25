using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data;

public class EmployeeContext : DataContext<EmployeeContext>
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasKey(e => e.EmployeeId);

        // Configure self-referencing relationship: Employee has many DirectReports and one Parent
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.DirectReports)
            .WithOne(p => p.Parent)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100); 
            entity.Property(e => e.LastName).HasMaxLength(100); 
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100); 
        });

        modelBuilder.Entity<ReportingStructure>()
            .HasOne(rs => rs.Employee)
            .WithMany()
            .IsRequired();
    }
}