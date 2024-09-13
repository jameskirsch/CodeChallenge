/* Summary: Small Demonstration pulling the Reporting Structure from the Backend, by grabbing the Root employee (in this case John Lennon)
   And displaying the hierarcy of the Employees and their direct and indirect reports.

   Possible setup requirements: Might need to allow access for dev certs to access https://localhost:5001/api/reporting .. HTTPS Redirecting is off though so might be ok
   1. To build the UI run in terminal: npm run build (might need to install node)
   2. Start the C# console application for the coding challenge
   3. Ensure port 5001 is available and run: npm start
   4. Upon starting should open browser with the JSON in network request pulling the reporting structure, but if you get connection refused, could be cert related on your local machine
   5. App.tsx should display a tree hierarchy of the Reporting Structure
   6. Can run tests by running: npm test
*/ 

import React, { useState, useEffect } from "react";
import { EmployeeDetailsComponent } from "../EmployeeDetails/EmployeeDetailsComponent"
import { ReportingStructure } from "../../types/ReportingStructure"

const reportingEndpoint = "https://localhost:5001/api/reporting";

const ReportingStructureComponent: React.FC = () => {
  const [reportingStructure, setReportingStructure] = useState<ReportingStructure | null>(null);

  const fetchReportingStructure = async () => {
    try {
      const rootEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
      const response = await fetch(`${reportingEndpoint}/${rootEmployeeId}`);
      const data = await response.json();
      setReportingStructure(data);
    } catch (error) {
      console.error("Error fetching reporting structure:", error);
    }
  };

  useEffect(() => {
    fetchReportingStructure();
  }, []);

  return (
    <div>
      <h2>Reporting Structure</h2>
      {reportingStructure ? (
        <div>
          <h3>Employee: {reportingStructure.Employee?.firstName} {reportingStructure.Employee?.lastName}</h3>
          <p>Position: {reportingStructure.Employee?.position}</p>
          <p>Department: {reportingStructure.Employee?.department}</p>
          <p>Number of Reports: {reportingStructure.numberOfReports}</p>

          {reportingStructure.Employee?.directReports && reportingStructure.Employee.directReports.length > 0 ? (
            <ul>
              {reportingStructure.Employee.directReports.map((directReport) => (
                <EmployeeDetailsComponent key={directReport.employeeId} employee={directReport} />
              ))}
            </ul>
          ) : (
            <p>This employee has no direct reports.</p>
          )}
        </div>
      ) : (
        <p>Loading Reporting Structure...</p>
      )}
    </div>
  );
};

export default ReportingStructureComponent;