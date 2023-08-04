//The Adapter design pattern is used to allow objects with incompatible interfaces to work together. It acts as a bridge between two incompatible interfaces,
//converting the interface of one class into another that clients expect. In the context of a front-end ERP system, you might use the Adapter pattern to integrate with external services or APIs that have different interfaces.
//Let's consider a real-life example of using the Adapter design pattern in a pure JavaScript front-end ERP system to integrate with a third-party project management API.
//Example: Integrating with Project Management API
//In an ERP system, you may want to integrate with a third-party project management API to fetch project data and display it within your system. However, 
//the API might have a different structure and methods compared to what your ERP system expects. The Adapter pattern can be used to adapt the API's interface to match your ERP system's interface.


// Third-party Project Management API
class ThirdPartyProjectAPI {
    constructor() {
        this.projects = [
            { id: 1, title: 'Project A', startDate: '2023-08-01', endDate: '2023-08-15' },
            { id: 2, title: 'Project B', startDate: '2023-09-01', endDate: '2023-09-30' },
        ];
    }

    getProjects() {
        return this.projects;
    }
}

// Target Interface for ERP System
class ProjectProvider {
    getProjects() { }
}

// Adapter for ThirdPartyProjectAPI
class ThirdPartyProjectAdapter extends ProjectProvider {
    constructor(api) {
        super();
        this.api = api;
    }

    getProjects() {
        const projects = this.api.getProjects();
        return projects.map(project => ({
            id: project.id,
            name: project.title,
            start: new Date(project.startDate),
            end: new Date(project.endDate),
        }));
    }
}

// ERP System
class ERPSystem {
    constructor(projectProvider) {
        this.projectProvider = projectProvider;
    }

    displayProjects() {
        const projects = this.projectProvider.getProjects();
        projects.forEach(project => {
            console.log(`Project: ${project.name}`);
            console.log(`Start Date: ${project.start.toDateString()}`);
            console.log(`End Date: ${project.end.toDateString()}`);
            console.log('----------------');
        });
    }
}

// Client Code
const thirdPartyAPI = new ThirdPartyProjectAPI();
const adapter = new ThirdPartyProjectAdapter(thirdPartyAPI);
const erpSystem = new ERPSystem(adapter);

erpSystem.displayProjects();


//ThirdPartyProjectAPI: Simulates a third-party project management API with a method to fetch projects.
//ProjectProvider: Defines the target interface that the ERP system expects.
//ThirdPartyProjectAdapter: Adapts the third - party API's interface to match the ProjectProvider interface. It converts the project data from the API into the format expected by the ERP system.
//ERPSystem: Represents your front - end ERP system that uses the ProjectProvider interface to fetch and display project data.
//By using the Adapter pattern, you can seamlessly integrate the third - party project management API into your ERP system without modifying the existing code.
//This is especially useful when dealing with external services or APIs that have different structures and methods than your internal system.