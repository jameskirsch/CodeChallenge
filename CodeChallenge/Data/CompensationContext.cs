using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data;

public class CompensationContext : DataContext<CompensationContext>
{
    public CompensationContext(DbContextOptions<CompensationContext> options) : base(options) { }

    public DbSet<Compensation> Compensations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compensation>()
            .HasOne<Employee>()
            .WithMany()
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Compensation>()
           .Property(c => c.Salary).HasColumnType("decimal(18,2)");
    }
}