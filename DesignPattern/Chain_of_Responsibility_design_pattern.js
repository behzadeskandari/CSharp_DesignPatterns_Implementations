//The Chain of Responsibility design pattern allows you to pass a request along a chain of handlers, where each handler decides whether to process the request or pass it to the next handler in the chain. 
//In the context of a front - end ERP system, you might use the Chain of Responsibility pattern to handle different types of user actions or events in a sequential manner.
//Let's explore a detailed example of using the Chain of Responsibility design pattern in a pure JavaScript front-end ERP system to handle user actions through a chain of event handlers with DOM manipulation.
//Example: Handling User Actions with Chain of Responsibility and DOM Manipulation
//In this example, we'll create a Chain of Responsibility for handling different types of user actions (e.g., clicking buttons, submitting forms) and manipulating the DOM accordingly

// Handler: AbstractHandler
class AbstractHandler {
    constructor(successor = null) {
        this.successor = successor;
    }

    setSuccessor(successor) {
        this.successor = successor;
    }

    handleRequest(action, event) {
        if (this.successor) {
            this.successor.handleRequest(action, event);
        }
    }
}

// Concrete Handlers
class ButtonClickHandler extends AbstractHandler {
    handleRequest(action, event) {
        if (action === 'buttonClick') {
            console.log('Button clicked! Handling action...');
            const buttonElement = event.target;
            buttonElement.style.backgroundColor = 'green';
        } else {
            super.handleRequest(action, event);
        }
    }
}

class FormSubmitHandler extends AbstractHandler {
    handleRequest(action, event) {
        if (action === 'formSubmit') {
            console.log('Form submitted! Handling action...');
            const formElement = event.target;
            const formData = new FormData(formElement);
            alert(`Form data submitted: ${JSON.stringify(Object.fromEntries(formData))}`);
        } else {
            super.handleRequest(action, event);
        }
    }
}

// Client Code
const buttonClickHandler = new ButtonClickHandler();
const formSubmitHandler = new FormSubmitHandler();

buttonClickHandler.setSuccessor(formSubmitHandler);

document.getElementById('btn-action').addEventListener('click', (event) => {
    buttonClickHandler.handleRequest('buttonClick', event);
});

document.getElementById('form-action').addEventListener('submit', (event) => {
    event.preventDefault();
    formSubmitHandler.handleRequest('formSubmit', event);
});


//AbstractHandler: The abstract handler class that defines the common structure for concrete handlers and provides a method to pass the request to the successor.
//Concrete Handlers(ButtonClickHandler, FormSubmitHandler): Concrete handler classes that implement specific handling logic for different actions(button click, form submit).
//Client code: Sets up the chain of responsibility by linking handlers together and assigns event listeners to specific DOM elements.
//By using the Chain of Responsibility pattern, you can create a sequence of handlers that process different types of user actions in a modular and extensible manner.
//This is useful in an ERP system where you might need to handle various user interactions, validations, or data submissions.
//In a real ERP system, you could apply the Chain of Responsibility pattern to more complex scenarios, such as handling different levels of user authentication, input validation, or routing based on user actions.
