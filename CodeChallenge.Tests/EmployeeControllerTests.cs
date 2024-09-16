using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Tests.Integration.Extensions;
using CodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeChallenge.Tests.Integration;

[TestClass]
public class EmployeeControllerTests
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
    public void CreateEmployee_Returns_Created()
    {
        // Arrange
        var employee = new Employee()
        {
            Department = "Complaints",
            FirstName = "Debbie",
            LastName = "Downer",
            Position = "Receiver",
        };

        var requestContent = new JsonSerialization().ToJson(employee);

        // Execute
        var postRequestTask = _httpClient.PostAsync("api/employee",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));
        var response = postRequestTask.Result;

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var newEmployee = response.DeserializeContent<Employee>();
        Assert.IsNotNull(newEmployee.EmployeeId);
        Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
        Assert.AreEqual(employee.LastName, newEmployee.LastName);
        Assert.AreEqual(employee.Department, newEmployee.Department);
        Assert.AreEqual(employee.Position, newEmployee.Position);
    }

    [TestMethod]
    public async Task GetEmployeeById_Returns_Ok()
    {
        // Arrange
        var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
        var expectedFirstName = "John";
        var expectedLastName = "Lennon";

        // Execute
        var getRequestTask = await _httpClient.GetAsync($"api/employee/{employeeId}");
        var response = getRequestTask;

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var employee = response.DeserializeContent<Employee>();
        Assert.AreEqual(expectedFirstName, employee.FirstName);
        Assert.AreEqual(expectedLastName, employee.LastName);
    }

    [TestMethod]
    public async Task UpdateEmployee_Returns_Ok()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeId = new Guid("03aa1462-ffa9-4978-901b-7c001562cf6f"),
            Department = "Engineering",
            FirstName = "Pete",
            LastName = "Best",
            Position = "Developer VI",
        };
            
        var requestContent = new JsonSerialization().ToJson(employee);
            
        // Execute
        var putResponse = await _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
        var updatedEmployee = putResponse.DeserializeContent<Employee>();

        Assert.AreEqual(employee.FirstName, updatedEmployee.FirstName);
        Assert.AreEqual(employee.LastName, updatedEmployee.LastName);
    }

    [TestMethod]
    public void UpdateEmployee_Returns_NotFound()
    {
        // Arrange
        var employee = new Employee()
        {
            EmployeeId = Guid.Empty,
            Department = "Music",
            FirstName = "Sunny",
            LastName = "Bono",
            Position = "Singer/Song Writer",
        };
        var requestContent = new JsonSerialization().ToJson(employee);

        // Execute
        var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));
        var response = postRequestTask.Result;

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}