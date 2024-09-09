﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using CodeChallenge.Models;
using CodeChallenge.Services;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CodeCodeChallenge.Tests.Integration.Extensions;
using System.Net;
using System.Threading.Tasks;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class ReportingStructureServiceTests
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
        public void GetReporting_Structure_By_EmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Act
            var getRequestTask = _httpClient.GetAsync($"api/reporting/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();

            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void Get_Reporting_Structure_By_EmployeeId_Returns_Full_Details()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfReports = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reporting/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();

            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(reportingStructure.NumberOfReports);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);
            Assert.IsNotNull(reportingStructure.Employee);
        }

        [TestMethod]
        public void Reporting_Structure_Service_Calculates_Report_Count_By_EmployeeId_Depth_Tree()
        {
            var employeeService = new Mock<IEmployeeService>().Object;
            var reportingStructureService = new ReportingStructureService(new NullLogger<ReportingStructureService>(), employeeService);

            // Arrange Tree Reporting Structure for Depth
            var reportingStructure = new ReportingStructure
            {
                Employee = EmployeeReportDepthTestTree
            };

            // Execute
            var actual = reportingStructureService.GetReportCount(reportingStructure.Employee);

            Assert.AreEqual(9,  actual);
        }

        [TestMethod]
        public void Reporting_Structure_Service_Calculates_Report_Count_By_EmployeeId_Wide_Tree()
        {
            var employeeService = new Mock<IEmployeeService>().Object;
            var reportingStructureService =
                new ReportingStructureService(new NullLogger<ReportingStructureService>(), employeeService);

            // Arrange Tree Reporting Structure for Width
            var reportingStructure = new ReportingStructure
            {
                Employee = EmployeeReportWidthTestTree
            };

            var actual = reportingStructureService.GetReportCount(reportingStructure.Employee);
            Assert.AreEqual(15, actual);
        }

        [TestMethod]
        public async Task Get_Reporting_Structure_With_Total_Reports_By_Employee_Id_By_Depth()
        {
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var employeeService = new Mock<IEmployeeService>();
            var expected = 9;

            employeeService.Setup(x => x.GetByIdWithDirectReports(It.IsAny<string>()))
                .ReturnsAsync(EmployeeReportDepthTestTree);

            var reportingStructureService =
                new ReportingStructureService(new NullLogger<ReportingStructureService>(), employeeService.Object);

            var result = await reportingStructureService.GetReportingStructureByEmployeeId(employeeId);
            var actual = result.NumberOfReports;

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Get_Reporting_Structure_With_Total_Reports_By_Employee_Id_By_Width()
        {
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var employeeService = new Mock<IEmployeeService>();
            var expected = 15;

            employeeService.Setup(x => x.GetByIdWithDirectReports(It.IsAny<string>()))
                .ReturnsAsync(EmployeeReportWidthTestTree);

            var reportingStructureService =
                new ReportingStructureService(new NullLogger<ReportingStructureService>(), employeeService.Object);

            var result = await reportingStructureService.GetReportingStructureByEmployeeId(employeeId);
            var actual = result.NumberOfReports;

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Request_Reporting_Structure_With_Fully_Filled_Out_Details()
        {
            var employee = new Employee
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };

            // Execute
             var response = 
                  _httpClient.GetAsync($"api/reporting-structure/get-reporting-structure-by-employee-id/{employee.EmployeeId}");

             var reportingStructureResult = response.Result;

            Assert.IsNotNull( response );
        }

        private static readonly Employee EmployeeReportDepthTestTree = new Employee
        {
            EmployeeId = Guid.NewGuid().ToString(),
            FirstName = "James",
            LastName = "Kirsch",

            DirectReports = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",

                            DirectReports = new List<Employee>
                            {
                                new Employee
                                {
                                    EmployeeId = Guid.NewGuid().ToString(),
                                    FirstName = "Sally",
                                    LastName = "Doe",
                                }
                            }
                        }
                    }
                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "Sam",
                            LastName = "Doe",

                            DirectReports = new List<Employee>
                            {
                                new Employee
                                {
                                    EmployeeId = Guid.NewGuid().ToString(),
                                    FirstName = "Tom",
                                    LastName = "Doe",
                                }
                            }
                        }
                    }
                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Jim",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "Elizabeth",
                            LastName = "Doe",

                            DirectReports = new List<Employee>
                            {
                                new Employee
                                {
                                    EmployeeId = Guid.NewGuid().ToString(),
                                    FirstName = "Martin",
                                    LastName = "Doe",
                                }
                            }
                        }
                    }
                }
            }
        };

        private static readonly Employee EmployeeReportWidthTestTree = new Employee
        {
            EmployeeId = Guid.NewGuid().ToString(),
            FirstName = "James",
            LastName = "Kirsch",

            DirectReports = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        }
                    }
                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "Sam",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        }
                    }
                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Jim",
                    LastName = "Doe",

                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "Elizabeth",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        },
                        new Employee
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            FirstName = "James",
                            LastName = "Doe",
                        }
                    }
                }
            }
        };
    }
}