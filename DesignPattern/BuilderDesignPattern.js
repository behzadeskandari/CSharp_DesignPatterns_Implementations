//The Builder design pattern is used to create complex objects step by step. It separates the construction of a complex object from its representation, allowing the same construction process to create different representations. 
//In the context of a front - end ERP system, you might use the Builder pattern to construct complex forms or configurations with multiple options.
//Let's consider a real-life example of using the Builder design pattern in a pure JavaScript front-end ERP system to create a complex configuration form for creating and managing new projects.
//Example: Creating Project Configuration Form
//In an ERP system, users might need to create projects with various settings such as project name, start date, end date, assigned employees, and project tasks.The Builder pattern can be used to create a flexible project configuration form.

// Builder
class ProjectBuilder {
    constructor() {
        this.project = new Project();
    }

    setName(name) {
        this.project.name = name;
    }

    setStartDate(startDate) {
        this.project.startDate = startDate;
    }

    setEndDate(endDate) {
        this.project.endDate = endDate;
    }

    assignEmployee(employee) {
        this.project.assignedEmployees.push(employee);
    }

    addTask(task) {
        this.project.tasks.push(task);
    }

    getResult() {
        return this.project;
    }
}

// Product
class Project {
    constructor() {
        this.name = '';
        this.startDate = null;
        this.endDate = null;
        this.assignedEmployees = [];
        this.tasks = [];
    }

    displayInfo() {
        console.log('Project:', this.name);
        console.log('Start Date:', this.startDate);
        console.log('End Date:', this.endDate);
        console.log('Assigned Employees:', this.assignedEmployees.join(', '));
        console.log('Tasks:', this.tasks.join(', '));
    }
}

// Director
class ProjectManager {
    constructor(builder) {
        this.builder = builder;
    }

    createProject(name, startDate, endDate) {
        this.builder.setName(name);
        this.builder.setStartDate(startDate);
        this.builder.setEndDate(endDate);
    }

    addEmployee(employee) {
        this.builder.assignEmployee(employee);
    }

    addTask(task) {
        this.builder.addTask(task);
    }
}

// Client Code
const builder = new ProjectBuilder();
const projectManager = new ProjectManager(builder);

projectManager.createProject('ERP System', new Date(), new Date('2023-12-31'));
projectManager.addEmployee('Alice Smith');
projectManager.addEmployee('Bob Johnson');
projectManager.addTask('Design UI');
projectManager.addTask('Implement Backend');

const project = builder.getResult();
project.displayInfo();



//ProjectBuilder: The builder class responsible for constructing a Project step by step.
//Project: The complex object(product) that we're building, with various attributes like name, start date, end date, assigned employees, and tasks.
//ProjectManager: The director class that uses the builder to create a project.It abstracts the process of building the project.
//This allows you to create projects with different configurations and various steps, providing flexibility and ease of use.
//The Builder pattern helps keep the client code clean and readable while allowing for the construction of complex objects with varying attributes.
//In a real ERP system, you could use a similar approach to build more intricate configurations for various entities, such as orders, invoices, or products, while keeping the construction process separate from the final representation.
