//In an ERP system, you might want to represent the organizational hierarchy of employees, teams, and departments. 
//The Composite pattern can help you create a unified structure to manage and display this hierarchy.

// Component
class OrganizationalUnit {
    constructor(name) {
        this.name = name;
        this.children = [];
    }

    add(unit) {
        this.children.push(unit);
    }

    remove(unit) {
        const index = this.children.indexOf(unit);
        if (index !== -1) {
            this.children.splice(index, 1);
        }
    }

    displayHierarchy(level = 0) {
        console.log(`${' '.repeat(level * 2)}${this.name}`);
        for (const child of this.children) {
            child.displayHierarchy(level + 1);
        }
    }
}

// Leaf: Employee
class Employee extends OrganizationalUnit {
    constructor(name, title) {
        super(name);
        this.title = title;
    }

    displayHierarchy(level) {
        console.log(`${' '.repeat(level * 2)}${this.name} (${this.title})`);
    }
}

// Client Code
const ceo = new Employee('John Doe', 'CEO');
const cto = new Employee('Alice Smith', 'CTO');
const cfo = new Employee('Bob Johnson', 'CFO');

const engineeringTeam = new OrganizationalUnit('Engineering Team');
const marketingTeam = new OrganizationalUnit('Marketing Team');

const seniorDev = new Employee('Eve Williams', 'Senior Developer');
const juniorDev = new Employee('David Brown', 'Junior Developer');
const marketingManager = new Employee('Grace Lee', 'Marketing Manager');

ceo.add(cto);
ceo.add(cfo);

cto.add(engineeringTeam);
cfo.add(marketingTeam);

engineeringTeam.add(seniorDev);
engineeringTeam.add(juniorDev);
marketingTeam.add(marketingManager);

ceo.displayHierarchy();


///OrganizationalUnit: The component class that represents both composite and leaf nodes. It defines methods to add, remove, and display hierarchy.
//Employee: A leaf class that extends OrganizationalUnit to represent individual employees.
//Client code: Creates an organizational hierarchy with a CEO, CTO, CFO, and various teams and employees.
//By using the Composite pattern, you can create a flexible and scalable way to represent complex hierarchical structures in your ERP system.
//It allows you to treat individual employees and teams uniformly while maintaining the hierarchy's integrity.
//Remember, in a real ERP system, you would likely have more attributes and interactions within the organizational hierarchy and across different parts of the system.