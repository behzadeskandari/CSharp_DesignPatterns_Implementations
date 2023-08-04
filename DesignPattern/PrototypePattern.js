///he Prototype design pattern in JavaScript involves creating objects by cloning an existing object, known as the prototype. This can be useful when you want to create multiple instances of objects that share some common properties and behavior.
//Let's consider a real-life example of using the Prototype design pattern in a pure JavaScript front-end ERP (Enterprise Resource Planning) system.
//Example: Creating Employee Objects
///Imagine you are developing an ERP system for managing employees within a company.Employees can have various attributes like name, employee ID, department, and roles.
//Instead of creating each employee object from scratch,
//you can use the Prototype pattern to create new instances based on a prototype employee.


// Prototype: Employee
const employeePrototype = {
    name: "",
    employeeID: "",
    department: "",
    roles: [],

    displayInfo: function () {
        console.log(`Name: ${this.name}, ID: ${this.employeeID}, Department: ${this.department}`);
        console.log("Roles:", this.roles.join(", "));
    },

    clone: function () {
        const clone = Object.create(this);
        clone.roles = this.roles.slice(); // Clone the roles array
        return clone;
    }
};

// Create prototype employee
const prototypeEmployee = Object.create(employeePrototype);
prototypeEmployee.name = "John Doe";
prototypeEmployee.employeeID = "123";
prototypeEmployee.department = "HR";
prototypeEmployee.roles.push("Employee");

// Create new employee instances using cloning
const employee1 = prototypeEmployee.clone();
employee1.name = "Alice Smith";
employee1.employeeID = "124";
employee1.roles.push("Manager");

const employee2 = prototypeEmployee.clone();
employee2.name = "Bob Johnson";
employee2.employeeID = "125";
employee2.roles.push("Developer");

// Display employee information
employee1.displayInfo();
console.log("----------------");
employee2.displayInfo();


//In this example, the employeePrototype object serves as the prototype for creating new employee instances. 
//The clone method creates a shallow copy of the prototype and allows you to customize properties for each employee.
//This approach is beneficial in an ERP system where employees share common attributes and behavior but have some unique properties.By using the Prototype design pattern, 
//you can efficiently create and manage multiple employee instances without duplicating code and memory.
