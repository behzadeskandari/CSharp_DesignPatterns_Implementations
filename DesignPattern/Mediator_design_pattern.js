//The Mediator design pattern is used to centralize communication between objects, reducing direct dependencies between them. In the context of a front-end ERP system, 
//you might use the Mediator pattern to manage interactions between different UI components, coordinating their actions and updates through a central mediator.
//Let's explore a detailed example of using the Mediator design pattern in a pure JavaScript front-end ERP system to manage communication between different UI components with DOM manipulation.
//Example: UI Component Mediator with DOM Manipulation
//In this example, we'll create a UI component mediator that coordinates interactions between different UI elements and updates the DOM accordingly.


// Mediator: UIMediator
class UIMediator {
    constructor() {
        this.components = [];
    }

    registerComponent(component) {
        this.components.push(component);
        component.setMediator(this);
    }

    notify(sender, event) {
        for (const component of this.components) {
            if (component !== sender) {
                component.onNotify(event);
            }
        }
    }
}

// Colleague: UIComponent
class UIComponent {
    constructor(name) {
        this.name = name;
        this.mediator = null;
    }

    setMediator(mediator) {
        this.mediator = mediator;
    }

    send(event) {
        console.log(`${this.name} sent ${event}`);
        this.mediator.notify(this, event);
    }

    onNotify(event) {
        console.log(`${this.name} received ${event}`);
        // Update the DOM or perform other actions based on the event
        const outputElement = document.getElementById('output');
        outputElement.innerHTML += `${this.name} received ${event}<br>`;
    }
}

// Client Code
const mediator = new UIMediator();

const button = new UIComponent('Button');
const input = new UIComponent('Input');
const select = new UIComponent('Select');

mediator.registerComponent(button);
mediator.registerComponent(input);
mediator.registerComponent(select);

document.getElementById('btn-action').addEventListener('click', () => {
    button.send('click');
});

document.getElementById('input-field').addEventListener('input', () => {
    input.send('input');
});

document.getElementById('select-menu').addEventListener('change', () => {
    select.send('change');
});

//UIMediator: The mediator class that manages interactions between UI components.
//UIComponent: The colleague class that communicates with the mediator and responds to events.
//Client code: Sets up UI components, registers them with the mediator, and adds event listeners to trigger interactions.
//By using the Mediator pattern, you can centralize the communication and coordination between UI components, reducing direct dependencies and making it easier to manage complex interactions.
//This is useful in an ERP system where you might have various UI components that need to work together based on user actions or events.
//In a real ERP system, you could apply the Mediator pattern to manage interactions between different parts of the UI, such as navigation menus,
//filters, and data displays, ensuring that changes in one component are properly communicated and reflected in others.