using System.Linq;
using CodeChallenge.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChallenge.Tests.Integration.Helpers;

public class TestServer : WebApplicationFactory<Program>
{
    private string DbName { get; }

    public TestServer(string dbName)
    {
        DbName = dbName;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the application's DbContext registration.
            var employeeContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EmployeeContext>));

            if (employeeContextDescriptor != null)
            {
                services.Remove(employeeContextDescriptor);
            }

            // Add a new DbContext registrations for the in-memory database.
            // TODO: Currently this only really benefits heavy interactions with the EmployeeContext, eventually would need to add in other contexts for true isolation.
            services.AddDbContext<EmployeeContext>(options =>
            {
                options.UseInMemoryDatabase(DbName);
            });
        });
    }
}