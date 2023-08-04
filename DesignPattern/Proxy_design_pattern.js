//The Proxy design pattern is used to control access to another object, acting as a placeholder or intermediary.
//In the context of a front - end ERP system, you might use the Proxy pattern to add an additional layer of control or functionality to an object, such as delaying its initialization or controlling access permissions.
//Let's explore a detailed example of using the Proxy design pattern in a pure JavaScript front-end ERP system to manage user access permissions for viewing sensitive information.
//Example: User Access Control Proxy with DOM Manipulation
//In this example, we'll use the Proxy pattern to create a proxy that controls access to sensitive employee information based on user roles, and we'll manipulate the DOM to display this information

// Real Subject: EmployeeService
class EmployeeService {
    constructor() {
        this.employees = [
            { id: 1, name: 'Alice Smith', role: 'Manager', salary: 80000 },
            { id: 2, name: 'Bob Johnson', role: 'Developer', salary: 60000 },
            { id: 3, name: 'Eve Williams', role: 'Designer', salary: 55000 },
        ];
    }

    getEmployeeDetails(id) {
        return this.employees.find(emp => emp.id === id);
    }
}

// Proxy: EmployeeServiceProxy
class EmployeeServiceProxy {
    constructor(employeeService, userRole) {
        this.employeeService = employeeService;
        this.userRole = userRole;
    }

    getEmployeeDetails(id) {
        const employee = this.employeeService.getEmployeeDetails(id)
            ;

        // Apply access control based on user role
        if (this.userRole === 'Manager' || this.userRole === 'Developer') {
            return employee;
        } else {
            return { id: employee.id, name: employee.name, role: 'Unauthorized' };
        }
    }
}

// Client Code
const realEmployeeService = new EmployeeService();
const employeeServiceProxy = new EmployeeServiceProxy(realEmployeeService, 'Developer');

const displayEmployeeDetails = (employeeId) => {
    const employee = employeeServiceProxy.getEmployeeDetails(employeeId);

    const employeeInfoElement = document.getElementById('employee-info');
    employeeInfoElement.innerHTML = `
    <p><strong>Name:</strong> ${employee.name}</p>
    <p><strong>Role:</strong> ${employee.role}</p>
    <p><strong>Salary:</strong> ${employee.salary}</p>
  `;
};

// Simulate user interaction
document.getElementById('btn-show-details').addEventListener('click', () => {
    const selectedEmployeeId = parseInt(document.getElementById('employee-id').value, 10);
    displayEmployeeDetails(selectedEmployeeId);
});


//EmployeeService: The real subject class that provides access to employee details.
//EmployeeServiceProxy: The proxy class that controls access to the EmployeeService based on user roles, allowing or denying access to sensitive information.
//Client code: Simulates user interaction by displaying employee details based on the user's input and the proxy's access control.
//By using the Proxy pattern, you can control access to sensitive information and add an extra layer of security or functionality without modifying the original service.
//This is useful in an ERP system where different users might have different levels of access to certain information.
//In a real ERP system, you could apply the Proxy pattern to various other scenarios, such as caching, lazy loading, or logging, where you want to control or enhance the behavior of certain objects or services.
