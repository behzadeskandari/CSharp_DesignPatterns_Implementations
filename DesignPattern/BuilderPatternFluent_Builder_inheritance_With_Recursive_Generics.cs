using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{

    /// <summary>
    /// Suppose your ERP system deals with complex products that can have multiple components and configurations. 
    /// The Fluent Builder Pattern with recursive generics can be used to create a fluent interface for building complex products.
    /// </summary>
    // ComplexProduct class representing a complex product in the ERP system
    public class ComplexProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> Components { get; set; }

        public ComplexProduct()
        {
            Components = new List<string>();
        }
    }

    // Fluent Builder class for building ComplexProduct
    public class ComplexProductBuilder : FluentBuilder<ComplexProduct, ComplexProductBuilder>
    {
        public ComplexProductBuilder WithComponent(string componentName)
        {
            Product.Components.Add(componentName);
            return this;
        }
    }

    // Generic Fluent Builder class with recursive generics
    public class FluentBuilder<TProduct, TBuilder> where TProduct : new() where TBuilder : FluentBuilder<TProduct, TBuilder>
    {
        protected TProduct Product;

        public FluentBuilder()
        {
            Product = new TProduct();
        }

        public TProduct Build()
        {
            return Product;
        }

        public TBuilder WithName(string name)
        {
            Product.Name = name;
            return (TBuilder)this;
        }

        public TBuilder WithPrice(decimal price)
        {
            Product.Price = price;
            return (TBuilder)this;
        }
    }

    class Program4
    {
        static void Main()
        {
            // Building a complex product using the Fluent Builder
            ComplexProduct product = new ComplexProductBuilder()
                .WithName("Super Computer")
                .WithPrice(1500)
                .WithComponent("CPU")
                .WithComponent("GPU")
                .WithComponent("RAM")
                .Build();

            Console.WriteLine("Product Name: " + product.Name);
            Console.WriteLine("Product Price: $" + product.Price);
            Console.WriteLine("Product Components: " + string.Join(", ", product.Components));
        }
    }
}


namespace DesignPattern.Builder
{
    // Configuration class representing a nested configuration in the ERP system
    public class Configuration
    {
        public string Name { get; set; }
        public List<Configuration> SubConfigurations { get; set; }

        public Configuration()
        {
            SubConfigurations = new List<Configuration>();
        }
    }

    // Fluent Builder class for building Configuration
    public class ConfigurationBuilder : FluentBuilder<Configuration, ConfigurationBuilder>
    {
        public ConfigurationBuilder AddSubConfiguration(Action<ConfigurationBuilder> builderAction)
        {
            var subBuilder = new ConfigurationBuilder();
            builderAction(subBuilder);
            Product.SubConfigurations.Add(subBuilder.Build());
            return this;
        }
    }

    class Program
    {
        static void Main()
        {
            // Building a nested configuration using the Fluent Builder
            Configuration configuration = new ConfigurationBuilder()
                .WithName("Main Configuration")
                .AddSubConfiguration(subConfigBuilder =>
                    subConfigBuilder.WithName("Sub Configuration 1")
                                    .AddSubConfiguration(subSubConfigBuilder =>
                                        subSubConfigBuilder.WithName("Sub-Sub Configuration 1"))
                                    .AddSubConfiguration(subSubConfigBuilder =>
                                        subSubConfigBuilder.WithName("Sub-Sub Configuration 2")))
                .AddSubConfiguration(subConfigBuilder =>
                    subConfigBuilder.WithName("Sub Configuration 2"))
                .Build();

            DisplayConfiguration(configuration, 0);
        }

        // Helper method to display the nested configuration
        static void DisplayConfiguration(Configuration config, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);
            Console.WriteLine(indent + "Configuration: " + config.Name);
            foreach (var subConfig in config.SubConfigurations)
            {
                DisplayConfiguration(subConfig, indentLevel + 1);
            }
        }
    }
}