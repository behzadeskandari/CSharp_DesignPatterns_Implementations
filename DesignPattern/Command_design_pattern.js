//The Command design pattern is used to encapsulate a request as an object, allowing for parameterization of clients with different requests, queuing of requests, and logging of the requests. 
//In the context of a front - end ERP system, you might use the Command pattern to encapsulate and decouple user actions or operations on the UI, such as button clicks or form submissions.
//Let's explore a detailed example of using the Command design pattern in a pure JavaScript front-end ERP system to handle user actions through command objects with DOM manipulation.
//Example: Handling User Actions with Command Pattern and DOM Manipulation
//In this example, we'll use the Command pattern to encapsulate and handle different user actions (e.g., clicking buttons, submitting forms) and manipulate the DOM accordingly.


// Command: AbstractCommand
class AbstractCommand {
    execute() { }
}

// Concrete Commands
class ButtonClickCommand extends AbstractCommand {
    constructor(receiver) {
        super();
        this.receiver = receiver;
    }

    execute() {
        console.log('Executing button click command...');
        this.receiver.handleClick();
    }
}

class FormSubmitCommand extends AbstractCommand {
    constructor(receiver) {
        super();
        this.receiver = receiver;
    }

    execute() {
        console.log('Executing form submit command...');
        this.receiver.handleSubmit();
    }
}

// Receiver: DOMManipulator
class DOMManipulator {
    handleClick() {
        const buttonElement = document.getElementById('btn-action');
        buttonElement.style.backgroundColor = 'green';
    }

    handleSubmit() {
        const formElement = document.getElementById('form-action');
        const formData = new FormData(formElement);
        alert(`Form data submitted: ${JSON.stringify(Object.fromEntries(formData))}`);
    }
}

// Invoker: UserActionInvoker
class UserActionInvoker {
    constructor() {
        this.commands = [];
    }

    addCommand(command) {
        this.commands.push(command);
    }

    executeCommands() {
        for (const command of this.commands) {
            command.execute();
        }
        this.commands = [];
    }
}

// Client Code
const domManipulator = new DOMManipulator();

const buttonClickCommand = new ButtonClickCommand(domManipulator);
const formSubmitCommand = new FormSubmitCommand(domManipulator);

const userActionInvoker = new UserActionInvoker();
userActionInvoker.addCommand(buttonClickCommand);
userActionInvoker.addCommand(formSubmitCommand);

document.getElementById('btn-action').addEventListener('click', () => {
    userActionInvoker.executeCommands();
});

document.getElementById('form-action').addEventListener('submit', (event) => {
    event.preventDefault();
    userActionInvoker.executeCommands();
});

//AbstractCommand: The abstract command class that defines the common structure for concrete commands.
//Concrete Commands(ButtonClickCommand, FormSubmitCommand): Concrete command classes that encapsulate specific actions and call methods on the receiver.
//DOMManipulator: The receiver class that performs the actual DOM manipulation actions.
//UserActionInvoker: The invoker class that holds and executes a list of commands.
//Client code: Sets up the invoker, commands, and event listeners to execute commands based on user actions.
//By using the Command pattern, you can encapsulate user actions as command objects, decoupling the UI from the actual logic and making it easier to manage and extend user interactions.
//This is useful in an ERP system where you want to handle various user actions and potentially support undo / redo functionality or command queuing.
//In a real ERP system, you could apply the Command pattern to more complex scenarios, such as executing database transactions, interacting with APIs, or managing a series of user interactions as part of a workflow.
