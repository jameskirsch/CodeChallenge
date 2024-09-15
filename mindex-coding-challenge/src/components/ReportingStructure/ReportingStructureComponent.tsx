/* Summary: Small Demonstration pulling the Reporting Structure from the Backend, by grabbing the Root employee (in this case John Lennon)
   And displaying the hierarcy of the Employees and their direct and indirect reports. View readme.md for more details on running.*/ 

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
          <h3>Employee: {reportingStructure.employee?.firstName} {reportingStructure.employee?.lastName}</h3>
          <p>Position: {reportingStructure.employee?.position}</p>
          <p>Department: {reportingStructure.employee?.department}</p>
          <p>Number of Reports: {reportingStructure.numberOfReports}</p>

          {reportingStructure.employee?.directReports && reportingStructure.employee.directReports.length > 0 ? (
            <ul>
              {reportingStructure.employee.directReports.map((directReport) => (
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