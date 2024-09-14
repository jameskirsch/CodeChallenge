# Mindex Coding Challenge - Reporting Structure

## Overview
This project is a small demonstration that pulls the Reporting Structure from the backend. It retrieves the root employee (in this case, **John Lennon**) and displays the hierarchy of employees, including their direct and indirect reports.
To view this file in preview mode use (CTRL + SHIFT + V) for better formatting.

## Prerequisites
- **Node.js**: Ensure you have Node.js installed on your machine. If not, you can download and install it from [here](https://nodejs.org/).
- **C# Console Application**: Start the backend C# application for the coding challenge. It should be running on port `5001`. If this port is already in use, update the port configuration as needed.

## Setup Instructions

1. **Install Dependencies**  
   In the terminal, navigate to the `UI` folder (e.g., `mindex-coding-challenge`), and run the following command to install necessary dependencies:
   ```bash
   npm install
   ```

2. **Build the Project**  
   To build the project, execute the following command from the mindex-coding-challenge/src directory:
   ```bash
   npm run build
   ```

3. **Start the Backend**  
   Start the C# console application. By default, it runs on `https://localhost:5001`. Ensure that no other applications are using this port.

4. **Start the Frontend**  
   Make sure port `3000` is available on your machine, then run:
   ```bash
   npm start
   ```
   This will launch the React application and open it in your browser.

5. **Handling HTTPS Certificates**  
   If you encounter a "connection refused" error when the frontend tries to access the backend API (e.g., `https://localhost:5001/api/reporting`), it could be due to certificate issues on your machine. You may need to approve development certificates for the HTTPS connection.

6. **Verify App Functionality**  
   Once the frontend starts, it should fetch the Reporting Structure and display a tree hierarchy of the employees in `App.tsx`. You can view the network requests to see the JSON data being pulled from the backend.

7. **Running Tests**  
   To run the Jest Test Runner, use the following command:
   ```bash
   npm test
   ```

## Notes
- **HTTPS Redirect**: The backend's HTTPS redirect is disabled, so access should be direct via HTTPS on port `5001`.
- **Port Conflicts**: If port `5001` or `3000` are already in use on your machine, you will need to adjust the port settings in your local environment.
