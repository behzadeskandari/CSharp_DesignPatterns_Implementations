//The Iterator design pattern provides a way to access elements of a collection sequentially without exposing the underlying representation.
//In the context of a front - end ERP system, you might use the Iterator pattern to traverse and manipulate a list of items in a controlled manner, updating the DOM as needed.
//Let's explore a detailed example of using the Iterator design pattern in a pure JavaScript front-end ERP system to iterate over a list of tasks and update the DOM.
//Example: Task List Iterator with DOM Manipulation
//In this example, we'll create a TaskList class that uses the Iterator pattern to manage a list of tasks and updates the DOM to display the tasks

// Iterator: TaskIterator
class TaskIterator {
    constructor(tasks) {
        this.tasks = tasks;
        this.index = 0;
    }

    hasNext() {
        return this.index < this.tasks.length;
    }

    next() {
        return this.tasks[this.index++];
    }
}

// Aggregate: TaskList
class TaskList {
    constructor() {
        this.tasks = [];
    }

    addTask(task) {
        this.tasks.push(task);
    }

    getIterator() {
        return new TaskIterator(this.tasks);
    }
}

// Client Code
const taskList = new TaskList();
taskList.addTask('Write report');
taskList.addTask('Prepare presentation');
taskList.addTask('Review code');

const taskIterator = taskList.getIterator();

const taskListElement = document.getElementById('task-list');

document.getElementById('btn-show-tasks').addEventListener('click', () => {
    taskListElement.innerHTML = ''; // Clear previous tasks

    while (taskIterator.hasNext()) {
        const task = taskIterator.next();
        const taskItem = document.createElement('li');
        taskItem.textContent = task;
        taskListElement.appendChild(taskItem);
    }
});


//TaskIterator: The iterator class that provides methods to traverse a list of tasks.
//TaskList: The aggregate class that manages the list of tasks and provides the iterator.
//Client code: Sets up a task list, creates an iterator, and updates the DOM to display the list of tasks when a button is clicked.
//By using the Iterator pattern, you can encapsulate the logic for iterating over a collection and provide a standardized way to access elements without exposing the underlying structure.This is useful in an ERP system where you might need to iterate over lists of items, such as tasks, messages, or orders, and update the UI accordingly.
//In a real ERP system, you could apply the Iterator pattern to various scenarios, such as iterating over lists of employees, products, or notifications, and updating the DOM to display the relevant information.