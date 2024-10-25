using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    public class DataContext<TContext> : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
