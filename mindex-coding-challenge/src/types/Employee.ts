export type Employee = {
    employeeId: string;
    firstName?: string;
    lastName?: string;
    position?: string;
    department?: string;
    directReports?: Employee[]
  };