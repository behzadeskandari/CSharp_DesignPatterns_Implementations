using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern.DynamicDecoratorComposition
{
    //dynamic decorator composition can be a powerful design pattern to add flexible and customizable features to various modules.
    //Let's consider a real-life example where we have an Order Processing module in an ERP system,
    //and we want to dynamically compose decorators to handle different order processing tasks such as validation, logging, and notifications.

    //1-Component Interface:Define an interface that represents the base component, which in our case is an order.
    public interface IOrder
    {
        void Process();
    }
    //2-Concrete Component: Implement the interface to create a concrete component representing an order.
    public class Order : IOrder
    {
        public void Process()
        {
            Console.WriteLine("Processing order...");
        }
    }
    //Decorator Base Class: Create an abstract base class for decorators that implements the component interface.

    public abstract class OrderDecorator : IOrder
    {
        protected IOrder _order;

        public OrderDecorator(IOrder order)
        {
            _order = order;
        }

        public abstract void Process();
    }
    ///Concrete Decorators:Implement concrete decorators that add specific processing tasks to orders.

    public class ValidationDecorator : OrderDecorator
    {
        public ValidationDecorator(IOrder order)
            : base(order)
        {
        }

        public override void Process()
        {
            // Perform validation logic before processing
            Console.WriteLine("Validating order...");
            _order.Process();
        }
    }

    public class LoggingDecorator : OrderDecorator
    {
        public LoggingDecorator(IOrder order)
            : base(order)
        {
        }

        public override void Process()
        {
            // Log order processing
            Console.WriteLine("Logging order processing...");
            _order.Process();
        }
    }

    public class NotificationDecorator : OrderDecorator
    {
        public NotificationDecorator(IOrder order)
            : base(order)
        {
        }

        public override void Process()
        {
            _order.Process();
            // Send notifications after processing
            Console.WriteLine("Sending notifications...");
        }
    }
    //Dynamic Decorator Composition: Let's use dynamic decorator composition to build and process orders with different combinations of decorators.

    class Program
    {
        static void Main(string[] args)
        {
            IOrder order = new Order();

            // Dynamically compose decorators
            order = new ValidationDecorator(order);
            order = new LoggingDecorator(order);
            order = new NotificationDecorator(order);

            // Process the order with decorators
            order.Process();
        }
    }
    //In this example, dynamic decorator composition allows us to add flexible and customizable features to the order processing module of an ERP system.
    //We dynamically compose decorators to handle validation, logging, and notifications, and the order processing tasks are executed in the desired sequence.
    //Dynamic decorator composition enables you to build complex processing pipelines in your ERP system while keeping the code modular and maintainable.
    //It allows you to add and remove decorators as needed without modifying the existing code, making your system more adaptable to changing requirements.
}
