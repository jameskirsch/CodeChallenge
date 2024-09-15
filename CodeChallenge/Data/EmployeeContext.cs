using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define Employee entity with a self-referencing relationship for DirectReports
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);  // Explicit primary key

            // Configure self-referencing relationship: Employee has many DirectReports and one Parent
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.DirectReports)
                .WithOne()  // No navigation property for Parent (handled only through ParentId)

                // TODO: EF Core warning about shadow properties and cyclic references arises because of the self-referencing relationship.
                // The issue is that EF Core automatically creates a shadow property (EmployeeId1) when it cannot properly map the 
                // relationship between the ParentId and the Employee. This is usually because EF Core cannot infer the navigation property correctly
                // when there's a self-referencing relationship.
                // 
                // Removing the ForeignKey configuration (HasForeignKey) still fetches all the data correctly, but the shadow property warning persists.
                // This warning can safely be ignored if the functionality works as expected. I am still seeking a way to fix that, or possibly change the model structure around.
                // I think I may be able to resolve this by utilizing a Recursive Projection Technique.
                // TODO: Investigate Recursive Projection Technique in regard to this warning message.

                //.HasForeignKey(e => e.ParentId)  // Foreign key is explicitly set to ParentId (optional)

                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes

            // Configure Compensation as related to Employee
            modelBuilder.Entity<Compensation>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define ReportingStructure entity and relationship with Employee
            modelBuilder.Entity<ReportingStructure>()
                .HasOne(rs => rs.Employee)
                .WithMany()
                .IsRequired();
        }
    }
}