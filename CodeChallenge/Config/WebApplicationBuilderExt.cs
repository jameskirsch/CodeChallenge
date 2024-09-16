using CodeChallenge.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeChallenge.Config;

public static class WebApplicationBuilderExt
{
    private const string DbName = "EmployeeDB";

    public static void UseEmployeeDb(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;

        if (env.IsDevelopment())
        {
            builder.Services.AddDbContext<EmployeeContext>(options =>
                options.UseInMemoryDatabase(DbName)
                    .UseLazyLoadingProxies());

            builder.Services.AddDbContext<CompensationContext>(options =>
                options.UseInMemoryDatabase(DbName)
                    .UseLazyLoadingProxies());
        }
        else
        {
            builder.Services.AddDbContext<EmployeeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies());

            builder.Services.AddDbContext<CompensationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies());
        }
    }
}