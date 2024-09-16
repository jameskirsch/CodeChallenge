using System;
using System.Threading.Tasks;
using CodeChallenge.Config.MapperProfiles;
using CodeChallenge.Data;
using CodeChallenge.Repositories;
using CodeChallenge.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeChallenge.Config;

public class App
{
    public async Task<WebApplication> Configure(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.UseEmployeeDb();

        ConfigureServices(builder.Services, builder.Environment);

        var app = builder.Build();
        await ConfigureMiddleware(app);

        return app;
    }

    private static void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", corsBuilder =>
            {

                if (env.IsDevelopment())
                {
                    corsBuilder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    var productionUrl = services.BuildServiceProvider().GetService<IConfiguration>()["ProductionUrl"];
                    if (string.IsNullOrEmpty(productionUrl))
                    {
                        throw new InvalidOperationException("Production URL must be set in the configuration.");
                    }

                    corsBuilder.WithOrigins("https://example.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });

        services.AddAutoMapper(typeof(EmployeeProfile));
        services.AddAutoMapper(typeof(ReportingStructureProfile));

        if (!env.IsDevelopment())
        {
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true; // requires submission to https://hstspreload.org/
            });
        }

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IReportingStructureService, ReportingStructureService>();
        services.AddScoped<ICompensationService, CompensationService>();
        services.AddScoped<ICompensationRepository, CompensationRepository>();

        services.AddControllers();
    }

    private static async Task ConfigureMiddleware(WebApplication app)
    {
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            await SeedEmployeeDbAsync(app);
        }

        app.UseExceptionHandler("/Error");
            
        if (!env.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseCors("AllowReactApp");
        app.UseAuthorization();
        app.MapControllers();
    }

    private static async Task SeedEmployeeDbAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeContext>();

        try
        {
            var seeder = new EmployeeDataSeeder(context);
            await seeder.Seed();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding the database: {ex.Message}");
        }
    }
}