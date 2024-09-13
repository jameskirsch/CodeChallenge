/* Summary: Small Demonstration pulling the Reporting Structure from the Backend, by grabbing the Root employee (in this case John Lennon)
   And displaying the hierarcy of the Employees and their direct and indirect reports.

   Possible setup requirements: Might need to allow access for dev certs to access https://localhost:5001/api/reporting .. HTTPS Redirecting is off though so might be ok
   1. To build the UI run in terminal: npm run build
   2. Start the C# console application for the coding challenge
   3. Ensure port 5001 is available and run: npm start
   4. Upon starting should open browser with the JSON in network request pulling the reporting structure, but if you get connection refused, could be cert related on your local machine
   5. App.tsx should display a tree hierarchy of the Reporting Structure
*/ 

import React from "react";
import ReportingStructureComponent from "./components/ReportingStructure/ReportingStructureComponent";

const App: React.FC = () => {
  return (
    <div>
      <h1>Employee Reporting</h1>
      <ReportingStructureComponent />
    </div>
  );
};

export default App;
