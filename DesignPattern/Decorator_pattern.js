//In an ERP system, you may want to enhance user notifications with extra features such as priority levels, timestamps, or message formatting. 
//The Decorator pattern can help you achieve this while keeping the notification system flexible.

// Component: Notification
class Notification {
    constructor(message) {
        this.message = message;
    }

    show() {
        console.log(this.message);
    }
}

// Concrete Component: BasicNotification
class BasicNotification extends Notification {
    constructor(message) {
        super(message);
    }
}

// Decorator: NotificationDecorator
class NotificationDecorator extends Notification {
    constructor(notification) {
        super();
        this.notification = notification;
    }

    show() {
        this.notification.show();
    }
}

// Concrete Decorator: PriorityDecorator
class PriorityDecorator extends NotificationDecorator {
    constructor(notification, priority) {
        super(notification);
        this.priority = priority;
    }

    show() {
        super.show();
        console.log(`Priority: ${this.priority}`);
    }
}

// Concrete Decorator: TimestampDecorator
class TimestampDecorator extends NotificationDecorator {
    constructor(notification) {
        super(notification);
    }

    show() {
        super.show();
        console.log(`Timestamp: ${new Date().toLocaleString()}`);
    }
}

// Client Code
const basicNotification = new BasicNotification('New order received.');

const priorityNotification = new PriorityDecorator(basicNotification, 'High');
const timestampedPriorityNotification = new TimestampDecorator(priorityNotification);

basicNotification.show();
console.log('----------------');
priorityNotification.show();
console.log('----------------');
timestampedPriorityNotification.show();


///Notification: The base component class representing a simple notification.
///BasicNotification: A concrete component that extends Notification.
//NotificationDecorator: The decorator class that extends Notification and wraps another Notification object.
//PriorityDecorator: A concrete decorator that adds priority information to notifications.
//TimestampDecorator: A concrete decorator that adds a timestamp to notifications.
//By using the Decorator pattern, you can dynamically add features to notifications while keeping the code modular and easy to extend.This is useful in an ERP system where notifications might need to carry additional information based on different contexts.
//In a real ERP system, you could apply the Decorator pattern to various UI elements, such as user messages, alerts, or data displays, to enhance their functionalities without directly modifying their original classes.