using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CodeChallenge.Data;
using CodeChallenge.Tests.Integration.Helpers;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Repositories.Interfaces;
using CodeChallenge.Services.Orchestrators;

namespace CodeChallenge.Tests.Integration;

[TestClass]
public class EmployeeServiceTests
{
    private static HttpClient _httpClient;
    private static TestServer _customWebApplicationFactory;

    [ClassInitialize]
    // Attribute ClassInitialize requires this signature
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static void SetupTest(TestContext context)
    {
        _customWebApplicationFactory = new TestServer("EmployeeServiceContextDb");
        _httpClient = _customWebApplicationFactory.CreateClient();
    }

    [ClassCleanup]
    public static async Task ClassCleanUp()
    {
        await _customWebApplicationFactory.DisposeAsync();
        _httpClient.Dispose();
    }

    [TestMethod]
    public async Task Ensure_Compensation_Created_And_Returned()
    {
        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        var mockCompensationRepository = new Mock<ICompensationRepository>();
        var mapper = new Mock<IMapper>();

        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            EmployeeId = employeeId,
            FirstName = "James",
            LastName = "Kirsch",
            Department = "Engineering",
            Position = "Developer"
        };

        var compensation = new Compensation
        {
            Salary = 2000.00M,
            EffectiveDate = DateTimeOffset.UtcNow,
            EmployeeId = employeeId
        };

        mockEmployeeRepository.Setup(x => x.AddAsync(It.IsAny<Employee>())).ReturnsAsync(employee);
        mockCompensationRepository.Setup(x => x.AddAsync(It.IsAny<Compensation>())).ReturnsAsync(compensation);
        mockEmployeeRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(employee);
        mockCompensationRepository.Setup(x => x.GetCompensationByEmployeeId(It.IsAny<Guid>())).ReturnsAsync(compensation);  
        
        // Initialize the service with the mock repository
        var compensationService = new CompensationService(new NullLogger<CompensationService>(), mockCompensationRepository.Object, mockEmployeeRepository.Object);
        var employeeService = new EmployeeService(new NullLogger<EmployeeService>(), mockEmployeeRepository.Object, mapper.Object);

        // Create employee record (employee record is required)
        var createdEmployee = await employeeService.AddAsync(employee);
        Assert.IsNotNull(createdEmployee);

        // Create the compensation record
        var createdCompensation = await compensationService.AddAsync(compensation);

        // Ensure the compensation is created
        Assert.IsNotNull(createdCompensation);

        // Retrieve the created compensation
        {
            var actualCompensationResult = await compensationService.GetCompensationByEmployeeId(compensation.EmployeeId);
            Assert.IsNotNull(actualCompensationResult);
        }
    }

    [TestMethod]
    public async Task Ensure_Compensation_Created_And_Persisted_InDatabase_Individually()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeId = Guid.NewGuid(),
            FirstName = "James",
            LastName = "Kirsch",
            Department = "Engineering",
            Position = "Developer"
        };

        var compensation = new Compensation
        {
            EmployeeId = employee.EmployeeId,
            Salary = 2000.00M,
            EffectiveDate = DateTimeOffset.UtcNow
        };

        // Use the same in-memory database for both EmployeeContext and CompensationContext
        const string sharedDatabaseName = "TestSharedDatabase";
        var employeeContext = GetEmployeeInMemoryContext(sharedDatabaseName);
        var compensationContext = GetCompensationInMemoryContext(sharedDatabaseName);

        var employeeRepository = new EmployeeRepository(new NullLogger<EmployeeRepository>(), employeeContext);
        var compensationRepository = new CompensationRepository(new NullLogger<CompensationRepository>(), compensationContext, employeeContext);

        var employeeService = new EmployeeService(new NullLogger<EmployeeService>(), employeeRepository, Mock.Of<IMapper>());
        var compensationService = new CompensationService(new NullLogger<CompensationService>(), compensationRepository, employeeRepository);

        // Act
        await employeeService.AddAsync(employee); // Save the employee to the in-memory DB
        await compensationService.AddAsync(compensation); // Save the compensation

        // Assert
        var actualPersistedCompensation = await compensationContext.Compensations
            .SingleOrDefaultAsync(c => c.EmployeeId == employee.EmployeeId);

        Assert.IsNotNull(actualPersistedCompensation);
        Assert.AreEqual(compensation.Salary, actualPersistedCompensation.Salary);
        Assert.AreEqual(compensation.EffectiveDate, actualPersistedCompensation.EffectiveDate);
    }

    private static EmployeeContext GetEmployeeInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<EmployeeContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new EmployeeContext(options);
    }

    private static CompensationContext GetCompensationInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<CompensationContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new CompensationContext(options);
    }
}