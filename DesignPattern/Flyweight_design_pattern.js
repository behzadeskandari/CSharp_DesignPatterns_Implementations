///The Flyweight design pattern is used to minimize memory usage by sharing data between similar objects. 
//In the context of a front - end ERP system, you might use the Flyweight pattern to optimize the memory usage of objects that have shared intrinsic properties while keeping their extrinsic properties separate.
//Let's explore a detailed example of using the Flyweight design pattern in a pure JavaScript front-end ERP system to manage and display different types of employees with shared properties.
//Example: Employee Management with Flyweight Pattern and DOM Manipulation
//In this example, we'll use the Flyweight pattern to manage different types of employees (e.g., managers, developers, designers) with shared intrinsic properties like their roles, 
//while keeping extrinsic properties like their names and departments separate.


// Flyweight: EmployeeType
class EmployeeType {
    constructor(role) {
        this.role = role;
    }
}

// Concrete Flyweights: Shared Intrinsic Properties
const managerType = new EmployeeType('Manager');
const developerType = new EmployeeType('Developer');
const designerType = new EmployeeType('Designer');

// Unshared Concrete Flyweight: Employee
class Employee {
    constructor(name, department, type) {
        this.name = name;
        this.department = department;
        this.type = type;
    }

    displayInfo() {
        console.log(`Name: ${this.name}, Department: ${this.department}, Role: ${this.type.role}`);
    }
}

// Factory for creating employees
class EmployeeFactory {
    constructor() {
        this.employees = [];
    }

    createEmployee(name, department, role) {
        let employeeType;

        switch (role.toLowerCase()) {
            case 'manager':
                employeeType = managerType;
                break;
            case 'developer':
                employeeType = developerType;
                break;
            case 'designer':
                employeeType = designerType;
                break;
            default:
                throw new Error(`Invalid role: ${role}`);
        }

        const existingEmployee = this.employees.find(emp => emp.type === employeeType);

        if (existingEmployee) {
            return new Employee(name, department, existingEmployee.type);
        } else {
            const newEmployee = new Employee(name, department, employeeType);
            this.employees.push(newEmployee);
            return newEmployee;
        }
    }
}

// Client Code
const employeeFactory = new EmployeeFactory();

const alice = employeeFactory.createEmployee('Alice Smith', 'HR', 'Manager');
const bob = employeeFactory.createEmployee('Bob Johnson', 'Engineering', 'Developer');
const eve = employeeFactory.createEmployee('Eve Williams', 'Design', 'Designer');
const charlie = employeeFactory.createEmployee('Charlie Brown', 'Marketing', 'Manager');

alice.displayInfo();
bob.displayInfo();
eve.displayInfo();
charlie.displayInfo();



//EmployeeType: The flyweight class representing shared intrinsic properties (roles) of employees.
//Concrete flyweights(managerType, developerType, designerType): Instances representing different roles, with shared intrinsic properties.
//Employee: The unshared flyweight class representing individual employees with extrinsic properties(name, department) and a reference to their type.
//EmployeeFactory: The factory class responsible for creating and managing employees, reusing existing flyweight instances if available.
//By using the Flyweight pattern, you can optimize memory usage by sharing intrinsic properties of similar objects while allowing for variations in their extrinsic properties.This is beneficial in an ERP system where different employees share common roles and properties but have distinct attributes.
//In a real ERP system, you could apply the Flyweight pattern to other scenarios where objects with shared intrinsic properties need to be efficiently managed while allowing for customization through extrinsic properties.



