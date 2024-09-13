import React from "react";
import { Employee } from "../../types/Employee";
import './EmployeeDetails.css';

export const EmployeeDetailsComponent: React.FC<{ employee: Employee }> = ({ employee }) => {
  return (
    <li key={employee.employeeId} className="employee">
      <p>
        {employee.firstName} {employee.lastName} - {employee.position} in {employee.department}
      </p>

      {/* Render direct reports recursively if there are any: I didn't choose something as complex as a DFS algorithm for this because I felt like it might be better
          managed with Pagination or Lazy Loading and just opted for a simple recursive solution: but could potentially revisit if that ever became a case */}
      {employee.directReports && employee.directReports.length > 0 && (
        <ul>
          {employee.directReports.map((report) => (
            <EmployeeDetailsComponent key={report.employeeId} employee={report} />
          ))}
        </ul>
      )}
    </li>
  );
};
