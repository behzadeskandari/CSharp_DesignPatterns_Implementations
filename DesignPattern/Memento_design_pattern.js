//The Memento design pattern is used to capture and restore an object's internal state without exposing its details. In the context of a front-end ERP system, 
//you might use the Memento pattern to save and restore the state of UI components or data, allowing users to undo or redo actions.
//Let's explore a detailed example of using the Memento design pattern in a pure JavaScript front-end ERP system to save and restore the state of a text input element with DOM manipulation.
//Example: Text Input State Management with Memento Pattern and DOM Manipulation
//In this example, we'll create a TextMemento class to capture and restore the state of a text input element, allowing users to save and undo changes.

// Memento: TextMemento
class TextMemento {
    constructor(text) {
        this.text = text;
    }
}

// Originator: TextInput
class TextInput {
    constructor() {
        this.inputElement = document.getElementById('text-input');
        this.history = [];
    }

    save() {
        const text = this.inputElement.value;
        const memento = new TextMemento(text);
        this.history.push(memento);
    }

    undo() {
        if (this.history.length > 0) {
            const previousState = this.history.pop();
            this.inputElement.value = previousState.text;
        }
    }
}

// Client Code
const textInput = new TextInput();

document.getElementById('btn-save').addEventListener('click', () => {
    textInput.save();
});

document.getElementById('btn-undo').addEventListener('click', () => {
    textInput.undo();
});


////TextMemento: The memento class that captures the state of the text input.
//TextInput: The originator class that manages the text input element and its history of states.
//Client code: Sets up the text input element, registers event listeners for saving and undoing changes, and interacts with the TextInput to manage its state.
//By using the Memento pattern, you can capture and restore the state of UI components or data, enabling undo and redo functionality in your ERP system.
//This is useful when users need to manipulate data or perform actions on the UI and then revert those changes.
//In a real ERP system, you could apply the Memento pattern to manage the state of more complex UI components or data structures, allowing users to navigate back and forth through their actions and changes.