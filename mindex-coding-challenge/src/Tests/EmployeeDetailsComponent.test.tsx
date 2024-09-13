import React from 'react';
import { render, screen } from '@testing-library/react';
import { EmployeeDetailsComponent } from '../components/EmployeeDetails/EmployeeDetailsComponent';

// simple test for now
describe('EmployeeDetails Component', () => {
  it('renders the employee details correctly', () => {
    render(<EmployeeDetailsComponent employee={mockEmployee} />);

    // Assertions
    expect(screen.getByText(/John Doe/)).toBeInTheDocument();
    expect(screen.getByText(/Developer/)).toBeInTheDocument();
    expect(screen.getByText(/Engineering/)).toBeInTheDocument();
  });
});

const mockEmployee = {
  employeeId: '1',
  firstName: 'John',
  lastName: 'Doe',
  position: 'Developer',
  department: 'Engineering',
  directReports: []
};