//The State design pattern is used to allow an object to change its behavior when its internal state changes.In the context of a front - end ERP system, 
//you might use the State pattern to manage the behavior of UI components based on their state, such as enabling or disabling buttons, changing colors, or updating content.
//Let's explore a detailed example of using the State design pattern in a pure JavaScript front-end ERP system to manage the state of a document editor UI with different editing modes using DOM manipulation.
//Example: Document Editor with State Pattern and DOM Manipulation
//In this example, we'll create a document editor that switches between different editing modes (e.g., read-only, edit) and updates the UI based on the current state.



// State: EditingState
class EditingState {
    constructor(editor) {
        this.editor = editor;
    }

    render() {
        console.log('Rendering editing UI...');
        const editorElement = document.getElementById('editor');
        editorElement.innerHTML = `
      <textarea id="editor-textarea" rows="6" cols="40"></textarea>
      <button id="btn-save">Save</button>
      <button id="btn-cancel">Cancel</button>
    `;

        const textarea = document.getElementById('editor-textarea');
        const btnSave = document.getElementById('btn-save');
        const btnCancel = document.getElementById('btn-cancel');

        btnSave.addEventListener('click', () => {
            this.editor.save(textarea.value);
            this.editor.setState(new ReadOnlyState(this.editor));
        });

        btnCancel.addEventListener('click', () => {
            this.editor.setState(new ReadOnlyState(this.editor));
        });
    }
}

// Concrete State: ReadOnlyState
class ReadOnlyState {
    constructor(editor) {
        this.editor = editor;
    }

    render() {
        console.log('Rendering read-only UI...');
        const editorElement = document.getElementById('editor');
        editorElement.innerHTML = `
      <div id="document-content"></div>
      <button id="btn-edit">Edit</button>
    `;

        const btnEdit = document.getElementById('btn-edit');
        btnEdit.addEventListener('click', () => {
            this.editor.setState(new EditingState(this.editor));
        });
    }
}

// Context: DocumentEditor
class DocumentEditor {
    constructor() {
        this.state = new ReadOnlyState(this);
    }

    setState(state) {
        this.state = state;
        this.render();
    }

    render() {
        this.state.render();
    }

    save(content) {
        console.log(`Saving content: ${content}`);
        const documentContent = document.getElementById('document-content');
        documentContent.textContent = content;
    }
}

// Client Code
const documentEditor = new DocumentEditor();
documentEditor.render();


///EditingState: The concrete state class that defines the behavior of the editor in editing mode.
//ReadOnlyState: The concrete state class that defines the behavior of the editor in read - only mode.
//DocumentEditor: The context class that manages the current state and delegates the behavior to the current state.
//Client code: Sets up the document editor, initializes it in read - only mode, and attaches event listeners to switch between editing modes.
//By using the State pattern, you can dynamically change the behavior of UI components based on their internal state.This is useful in an 
//ERP system where you might have different modes or states for different parts of the user interface, such as read - only views, edit views, or confirmation dialogs.
//In a real ERP system, you could apply the State pattern to various scenarios, such as managing different steps in a multi - step form, controlling the behavior of interactive components, or handling different modes of interaction in a user dashboard.