//The Observer design pattern is used to define a dependency between objects, so that when one object changes its state, 
//all its dependents are notified and updated automatically.In the context of a front - end ERP system, you might use the Observer pattern to keep UI components synchronized with changes in data or state.
//Let's explore a detailed example of using the Observer design pattern in a pure JavaScript front-end ERP system to synchronize the state of a clock with multiple display elements using DOM manipulation.
//Example: Clock and Display Synchronization with Observer Pattern and DOM Manipulation
//In this example, we'll create a clock that updates its time and notifies multiple display elements to reflect the current time.
// Observer: DisplayElement
class DisplayElement {
    constructor() {
        this.element = null;
    }

    setElement(element) {
        this.element = element;
    }

    update(time) {
        // Update the display element with the new time
        this.element.textContent = time;
    }
}

// Subject: Clock
class Clock {
    constructor() {
        this.observers = [];
    }

    addObserver(observer) {
        this.observers.push(observer);
    }

    removeObserver(observer) {
        const index = this.observers.indexOf(observer);
        if (index !== -1) {
            this.observers.splice(index, 1);
        }
    }

    notifyObservers() {
        const currentTime = new Date().toLocaleTimeString();
        for (const observer of this.observers) {
            observer.update(currentTime);
        }
    }

    start() {
        setInterval(() => {
            this.notifyObservers();
        }, 1000);
    }
}

// Client Code
const clock = new Clock();

const display1 = new DisplayElement();
const display2 = new DisplayElement();

display1.setElement(document.getElementById('display1'));
display2.setElement(document.getElementById('display2'));

clock.addObserver(display1);
clock.addObserver(display2);

clock.start();


//DisplayElement: The observer class that defines the interface for display elements that need to be updated with new time.
//Clock: The subject class that manages observers and notifies them of state changes(time updates).
//Client code: Sets up the clock, creates display elements, attaches them as observers to the clock, and starts the clock.
//By using the Observer pattern, you can ensure that UI components are automatically updated when the underlying data or state changes.
//This is useful in an ERP system where you might have multiple UI elements that need to stay synchronized with changing data or events.
//In a real ERP system, you could apply the Observer pattern to various scenarios, such as updating charts, graphs, or data tables whenever relevant data changes, ensuring a real - time representation of the system's state.