using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern.Decorator
{
    // Legacy API
    public class LegacyExternalApi
    {
        public string GetProductInfoFromApi(string productId)
        {
            // Make API call and return JSON response
            return $"{{\"id\": \"{productId}\", \"name\": \"Legacy Product\", \"price\": 100}}";
        }
    }

    // Adapter for Legacy API
    public class ExternalApiAdapter
    {
        private readonly LegacyExternalApi _legacyApi;

        public ExternalApiAdapter(LegacyExternalApi legacyApi)
        {
            _legacyApi = legacyApi;
        }

        public ProductInfo GetProductInfo(string productId)
        {
            string jsonInfo = _legacyApi.GetProductInfoFromApi(productId);
            // Convert JSON to ProductInfo object
            // ...
            ProductInfo productInfo = new ProductInfo();


            return productInfo;

        }
    }

    public class ProductInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // Decorator base class
    public abstract class ProductDecorator : ProductInfo
    {
        protected ProductInfo _product;

        public ProductDecorator(ProductInfo product)
        {
            _product = product;
        }
    }

    // Decorator to add VAT calculation
    public class VatDecorator : ProductDecorator
    {
        public VatDecorator(ProductInfo product) : base(product)
        {
            Price *= 1.15M; // Applying 15% VAT
        }
    }

    // Decorator to add discount calculation
    public class DiscountDecorator : ProductDecorator
    {
        public DiscountDecorator(ProductInfo product) : base(product)
        {
            Price *= 0.9M; // Applying 10% discount
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var legacyApi = new LegacyExternalApi();
            var adapter = new ExternalApiAdapter(legacyApi);

            // Retrieve product information from the legacy API
            ProductInfo productInfo = adapter.GetProductInfo("123");

            // Apply decorators
            productInfo = new VatDecorator(productInfo);
            productInfo = new DiscountDecorator(productInfo);

            // Display the final product information
            Console.WriteLine($"Product ID: {productInfo.Id}");
            Console.WriteLine($"Product Name: {productInfo.Name}");
            Console.WriteLine($"Product Price: {productInfo.Price:C}");
        }
    }
}



namespace DesignPattern.Decorator2
{
    //1-Component Interface: //Define an interface that represents the base component, which in our case is the inventory item.

    public interface IInventoryItem
    {
        string Name { get; }
        decimal Price { get; }
    }

    //2-Concrete Component:  Implement the interface to create a concrete component representing a basic inventory item.

    public class InventoryItem : IInventoryItem
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public InventoryItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }

    //3-Decorator Base Class: Create an abstract base class for decorators that implements the component interface.

    public abstract class InventoryDecorator : IInventoryItem
    {
        protected IInventoryItem _item;

        public InventoryDecorator(IInventoryItem item)
        {
            _item = item;
        }

        public abstract string Name { get; }
        public abstract decimal Price { get; }
    }
    ///4- Concrete Decorators: Implement concrete decorators that extend the functionality of the inventory items.
    public class ExpirationDateDecorator : InventoryDecorator
    {
        private DateTime _expirationDate;

        public ExpirationDateDecorator(IInventoryItem item, DateTime expirationDate)
            : base(item)
        {
            _expirationDate = expirationDate;
        }

        public override string Name => $"{_item.Name} (Expires on {_expirationDate:yyyy-MM-dd})";
        public override decimal Price => _item.Price;
    }

    public class DiscountDecorator : InventoryDecorator
    {
        private decimal _discountPercentage;

        public DiscountDecorator(IInventoryItem item, decimal discountPercentage)
            : base(item)
        {
            _discountPercentage = discountPercentage;
        }

        public override string Name => _item.Name;
        public override decimal Price => _item.Price * (1 - _discountPercentage / 100);
    }
    //5-Using the Decorators: Let's use the decorators in the ERP system to enhance the inventory items.

    class Program
    { 
        static void Main(string[] args)
        {
            IInventoryItem item = new InventoryItem("Product A", 100);

            // Apply decorators
            item = new ExpirationDateDecorator(item, DateTime.Today.AddYears(1));
            item = new DiscountDecorator(item, 10);

            // Display enhanced item information
            Console.WriteLine($"Item: {item.Name}");
            Console.WriteLine($"Price: {item.Price:C}");
        }
    }
}





namespace DesignPattern.Decorator3
{
    /// the Decorator pattern can also be applied in the context of dependency injection to enhance the behavior of services while maintaining the flexibility and modularity of the application.
    /// Let's consider a real-life example where we have an Order Processing module in an ERP system, 
    /// and we want to apply decorators using dependency injection to add validation, logging, and notification functionalities.
    ///Here's how the Decorator pattern can be applied in dependency injection within a C# ERP system:
    ///

    //1-Service Interface: Define an interface that represents the base service, which in our case is an order processing service.
    public interface IOrderProcessingService
    {
        void ProcessOrder(Order order);
    }


    ///2-Concrete Service:Implement the interface to create a concrete service for processing orders
    ///
    public class OrderProcessingService : IOrderProcessingService
    {
        public void ProcessOrder(Order order)
        {
            // Perform order processing logic
            Console.WriteLine($"Processing order {order.OrderId}...");
        }
    }

    //3-Decorator Base Class:Create an abstract base class for decorators that implements the service interface.
    public abstract class OrderProcessingDecorator : IOrderProcessingService
    {
        protected IOrderProcessingService _service;

        public OrderProcessingDecorator(IOrderProcessingService service)
        {
            _service = service;
        }

        public abstract void ProcessOrder(Order order);
    }

    ///4-Concrete Decorators:Implement concrete decorators that add specific functionalities to the order processing service.
    ///
    public class ValidationDecorator : OrderProcessingDecorator
    {
        public ValidationDecorator(IOrderProcessingService service)
            : base(service)
        {
        }

        public override void ProcessOrder(Order order)
        {
            // Perform validation logic before processing
            Console.WriteLine($"Validating order {order.OrderId}...");
            _service.ProcessOrder(order);
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
    }

    public class LoggingDecorator : OrderProcessingDecorator
    {
        public LoggingDecorator(IOrderProcessingService service)
            : base(service)
        {
        }

        public override void ProcessOrder(Order order)
        {
            // Log order processing
            Console.WriteLine($"Logging order processing for order {order.OrderId}...");
            _service.ProcessOrder(order);
        }
    }

    public class NotificationDecorator : OrderProcessingDecorator
    {
        public NotificationDecorator(IOrderProcessingService service)
            : base(service)
        {
        }

        public override void ProcessOrder(Order order)
        {
            _service.ProcessOrder(order);
            // Send notifications after processing
            Console.WriteLine($"Sending notifications for order {order.OrderId}...");
        }
    }
    //5-Dependency Injection and Composition Root: Use a dependency injection container to dynamically compose the decorators and resolve the decorated service.
    class Program
    {
        static void Main(string[] args)
        {
            var container = new DependencyInjectionContainer();
            container.Register<IOrderProcessingService, OrderProcessingService>();

            // Dynamically compose decorators using dependency injection
            container.RegisterDecorator<IOrderProcessingService, ValidationDecorator>();
            container.RegisterDecorator<IOrderProcessingService, LoggingDecorator>();
            container.RegisterDecorator<IOrderProcessingService, NotificationDecorator>();

            // Resolve and use the decorated service
            var orderProcessingService = container.Resolve<IOrderProcessingService>();
            var order = new Order { OrderId = "123" };
            orderProcessingService.ProcessOrder(order);
        }
    }
}
 