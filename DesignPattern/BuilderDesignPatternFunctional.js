//The Bridge design pattern is used to decouple abstraction from implementation, allowing both to vary independently.In a functional programming style, you can achieve a similar decoupling using higher - order functions and function composition.Let's explore a detailed example of using the Bridge design pattern with functional programming in a pure JavaScript front-end ERP system, incorporating DOM manipulation.
//Example: Bridge Design Pattern with Functional Programming and DOM Manipulation
//In this example, we'll create a bridge between different notification methods (e.g., email, SMS) and message types (e.g., critical, normal) using functional programming concepts.

// Abstraction: Notification
const createNotification = (sendMethod) => (message, recipient) => {
    console.log(`Sending "${message}" to ${recipient} via ${sendMethod}`);
    // Simulated sending logic
};

// Implementations: Send Methods
const emailSender = (message, recipient) => {
    // Simulated email sending logic
    createEmailNotification(message, recipient);
};

const smsSender = (message, recipient) => {
    // Simulated SMS sending logic
    createSmsNotification(message, recipient);
};

// Abstraction: Message Type
const createCriticalNotification = (send) => (message) => send(message, 'Critical Recipient');

const createNormalNotification = (send) => (message) => send(message, 'Normal Recipient');

// Client Code
const criticalEmailNotification = createCriticalNotification(emailSender);
const normalSmsNotification = createNormalNotification(smsSender);

const sendButton = document.getElementById('send-btn');
sendButton.addEventListener('click', () => {
    const messageElement = document.getElementById('message');
    const messageTypeElement = document.getElementById('message-type');

    const message = messageElement.value;
    const messageType = messageTypeElement.value;

    let notification;

    if (messageType === 'critical') {
        notification = criticalEmailNotification;
    } else if (messageType === 'normal') {
        notification = normalSmsNotification;
    }

    notification(message);
});



//We define higher - order functions(createNotification, createCriticalNotification, createNormalNotification) to create abstractions and bridge different implementations.
//We create separate implementations(emailSender, smsSender) for sending notifications.
//The client code sets up event listeners to handle notification requests from the user, selects the appropriate bridge(abstraction) based on the message type, and invokes the notification.
//By using the Bridge pattern with functional programming concepts, you achieve decoupling between abstraction and implementation in a modular and composable manner.Functional programming allows you to encapsulate behavior and adapt different implementations while keeping the overall design clean and maintainable.
///In a real ERP system, you could apply this approach to other scenarios, such as integrating different payment gateways, managing authentication methods, or handling data storage options with various implementations while maintaining a consistent interface