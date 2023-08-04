using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    //The Bridge Design Pattern is used to separate abstraction from implementation, allowing both to evolve independently.
    //In an ERP (Enterprise Resource Planning) system, this pattern can be applied to handle different features and functionalities while keeping the implementation details separate.
    //Let's consider a detailed example of the Bridge Design Pattern in a C# ERP system
    //Suppose our ERP system needs to handle different types of notifications(e.g., email, SMS) and different priority levels(e.g., high, medium, low) for notifying users about various events


    //1-Define the NotificationAbstraction (Abstraction) Interface:
    public interface INotification
    {
        void Notify(string message);
    }


    //2-Create Concrete Implementations for Notifications:
    // Concrete Implementations for Notification: Email Notification
    public class EmailNotification : INotification
    {
        public void Notify(string message)
        {
            Console.WriteLine($"Sending email notification: {message}");
            // Actual email sending implementation...
        }
    }

    // Concrete Implementations for Notification: SMS Notification
    public class SmsNotification : INotification
    {
        public void Notify(string message)
        {
            Console.WriteLine($"Sending SMS notification: {message}");
            // Actual SMS sending implementation...
        }
    }

    //3-Create Abstraction Classes for Different Priority Levels:
    // Abstraction: NotificationPriority
    public abstract class NotificationPriority
    {
        protected INotification notification;

        public NotificationPriority(INotification notification)
        {
            this.notification = notification;
        }

        public abstract void SendNotification(string message);
    }

    //4-Integrate Event Handlers into the ERP System:
    public class ERPSystem23
    {
        public event EventHandler<string> HighPriorityEvent;
        public event EventHandler<string> MediumPriorityEvent;
        public event EventHandler<string> LowPriorityEvent;

        public void NotifyUser(string message, NotificationPriority priority)
        {
            priority.SendNotification(message);
        }

        public void OnHighPriority(string message)
        {
            HighPriorityEvent?.Invoke(this, message);
        }

        public void OnMediumPriority(string message)
        {
            MediumPriorityEvent?.Invoke(this, message);
        }

        public void OnLowPriority(string message)
        {
            LowPriorityEvent?.Invoke(this, message);
        }
    }

    class Program56
    {
        public static void Main(string[] args)
        {
            // Create concrete notification implementations
            INotification emailNotification = new EmailNotification();
            INotification smsNotification = new SmsNotification();

            // Create concrete priority implementations
            NotificationPriority highPriority = new HighPriority(emailNotification);
            NotificationPriority mediumPriority = new MediumPriority(smsNotification);
            NotificationPriority lowPriority = new LowPriority(emailNotification);

            // Integrate Bridge Design Pattern with event handlers in ERP system
            ERPSystem23 erpSystem = new ERPSystem23();

            erpSystem.HighPriorityEvent += (sender, message) =>
            {
                Console.WriteLine($"High Priority Event: {message}");
                highPriority.SendNotification(message);
            };

            erpSystem.MediumPriorityEvent += (sender, message) =>
            {
                Console.WriteLine($"Medium Priority Event: {message}");
                mediumPriority.SendNotification(message);
            };

            erpSystem.LowPriorityEvent += (sender, message) =>
            {
                Console.WriteLine($"Low Priority Event: {message}");
                lowPriority.SendNotification(message);
            };

            erpSystem.NotifyUser("Critical issue detected!", highPriority);
            erpSystem.NotifyUser("Meeting reminder", mediumPriority);
            erpSystem.NotifyUser("Weekly newsletter", lowPriority);
        }
    }

    ///In this example, we integrated the Bridge Design Pattern along with event handlers in a C# ERP system. 
    ///The ERPSystem class uses events to handle notifications of different priority levels. 
    ///The Bridge pattern separates the notification implementation from the priority level, and event handlers are used to send notifications and perform related actions. 
    ///This design enables the ERP system to handle notifications with different priorities using a flexible and extensible architecture.
}
