﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CodeChallenge.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public async Task Ensure_Compensation_Created_And_Returned()
        {
            var mockEmployeeContextRepo = new Mock<IEmployeeRepository>();

            Compensation addedCompensation = null;
            Employee addedEmployee = null;

            // spy on Add method 
            mockEmployeeContextRepo.Setup(x => x.AddAsync(It.IsAny<Compensation>()))
                .Callback<Compensation>(compensation => addedCompensation = compensation)
                .ReturnsAsync((Compensation comp) => comp);

            mockEmployeeContextRepo.Setup(x => x.AddAsync(It.IsAny<Employee>()))
                .Callback<Employee>(emp => addedEmployee = emp)
                .ReturnsAsync((Employee emp) => emp);

            mockEmployeeContextRepo.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);  // SaveAsync should return a completed task

            mockEmployeeContextRepo.Setup(x => x.GetById(It.IsAny<string>()))
                .ReturnsAsync((string _) => addedEmployee);
            mockEmployeeContextRepo.Setup(x => x.GetCompensationByEmployeeId(It.IsAny<string>()))
                .ReturnsAsync((string _) => addedCompensation);  // Return the compensation by employeeId

            // Create test employee and compensation
            var employeeId = Guid.NewGuid().ToString();
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
                EmployeeId = employeeId,
                Salary = 2000.00M,
                EffectiveDate = DateTimeOffset.UtcNow
            };

            // Initialize the service with the mock repository
            var employeeService = new EmployeeService(new NullLogger<EmployeeService>(), mockEmployeeContextRepo.Object);

            // Create employee record (employee record is required)
            var createdEmployee = employeeService.Create(employee);
            Assert.IsNotNull(createdEmployee);

            // Create the compensation record
            var createdCompensation = await employeeService.Create(compensation);

            // Ensure the compensation is created
            Assert.IsNotNull(createdCompensation);

            // Retrieve the created compensation
            var actualCompensationResult = await employeeService.GetCompensationByEmployeeId(compensation.Employee.EmployeeId);

            Assert.IsNotNull(actualCompensationResult);
        }

        [TestMethod]
        public async Task Ensure_Compensation_Created_And_Persisted_InDatabase()
        {
            // Create an in memory database for testing
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(databaseName: "TestEmployeeDatabase")
                .Options;

            // Use the in-memory database for the EmployeeContext
            await using var context = new EmployeeContext(options);
            var employeeService = new EmployeeService(new NullLogger<EmployeeService>(), 
                new EmployeeRepository(new NullLogger<IEmployeeRepository>(), context));

            // Create a test employee and compensation
            var employeeId = Guid.NewGuid().ToString();

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
                EmployeeId = employeeId,
                Salary = 2000.00M,
                EffectiveDate = DateTimeOffset.UtcNow
            };

            // Add the Employee (need to have an employee)
            await employeeService.Create(employee);
            
            // Add the Compensation
            await employeeService.Create(compensation);

            // Assert
            // Ensure the compensation was persisted to the database
            var actualPersistedCompensation = await context.Compensation.SingleOrDefaultAsync(c => c.EmployeeId == employeeId);
            
            Assert.IsNotNull(actualPersistedCompensation);
            Assert.AreEqual(compensation.Salary, actualPersistedCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, actualPersistedCompensation.EffectiveDate);
        }
    }
}
