using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Tests.Integration.Extensions;
using CodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CodeChallenge.Tests.Integration;

[TestClass]
public class CompensationControllerTests
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
    public async Task CreateCompensation_Returns_Created()
    {
        var employeeId = new Guid("16a596ae-edd3-4847-99fe-c4518e82c86f");

        // Arrange
        var compensation = new Compensation
        {
            CompensationId = Guid.NewGuid(),
            EmployeeId = employeeId,
            Salary = 2000.00M,
            EffectiveDate = DateTimeOffset.UtcNow
        };

        var requestContent =
            new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

        // Execute
        var response = await _httpClient.PostAsync("api/compensation", requestContent);
        var newCompensation = response.DeserializeContent<Compensation>();

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(compensation.EmployeeId);
        Assert.AreEqual(compensation.Salary, newCompensation.Salary);
    }
}