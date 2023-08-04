

//The Factory Method design pattern is used to create objects without specifying the exact class of the object that will be created. This pattern is useful when you want to delegate the responsibility of object creation to subclasses,
//allowing for more flexibility and extensibility.
///Let's continue with our ERP system example and demonstrate how the Factory Method design pattern can be applied in the front-end using a detailed example.
//Example: Creating Employees with Different Roles
//In an ERP system, employees can have various roles such as Managers, Developers, 
//and Designers.Each role might have different attributes and behavior associated with it.We'll use the Factory Method pattern to create employee instances based on their roles.


// Base Employee class
class Employee {
    constructor(name, employeeID, department) {
        this.name = name;
        this.employeeID = employeeID;
        this.department = department;
    }

    displayInfo() {
        console.log(`Name: ${this.name}, ID: ${this.employeeID}, Department: ${this.department}`);
    }
}

// Concrete employee classes
class Manager extends Employee {
    constructor(name, employeeID, department) {
        super(name, employeeID, department);
        this.role = "Manager";
    }
}

class Developer extends Employee {
    constructor(name, employeeID, department) {
        super(name, employeeID, department);
        this.role = "Developer";
    }
}

class Designer extends Employee {
    constructor(name, employeeID, department) {
        super(name, employeeID, department);
        this.role = "Designer";
    }
}

// Factory Method pattern
class EmployeeFactory {
    createEmployee(role, name, employeeID, department) {
        switch (role.toLowerCase()) {
            case "manager":
                return new Manager(name, employeeID, department);
            case "developer":
                return new Developer(name, employeeID, department);
            case "designer":
                return new Designer(name, employeeID, department);
            default:
                throw new Error(`Invalid role: ${role}`);
        }
    }
}

// Client code
const factory = new EmployeeFactory();

const employee1 = factory.createEmployee("manager", "Alice Smith", "123", "HR");
const employee2 = factory.createEmployee("developer", "Bob Johnson", "124", "Engineering");
const employee3 = factory.createEmployee("designer", "Eve Williams", "125", "Design");

employee1.displayInfo();
employee2.displayInfo();
employee3.displayInfo();




//n this example, we have a base Employee class and concrete subclasses (Manager, Developer, and Designer) that inherit from it. The EmployeeFactory class encapsulates the object creation logic based on the specified role.
//The Factory Method pattern allows you to add new roles(subclasses) and extend the system without modifying the existing code.This is particularly useful in an ERP system where new roles might be introduced over time.
//Please note that this is a simplified example.In a real ERP system, you would likely have more complex attributes and behaviors associated with employees and their roles.Additionally, 
//the example focuses on the Factory Method pattern itself; an ERP system would involve more interactions and features between different components
//
//
//
//

