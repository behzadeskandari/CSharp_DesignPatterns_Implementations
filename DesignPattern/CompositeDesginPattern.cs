using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern.Composite
{
    internal class CompositeDesginPattern
    {
    }

    // we'll consider an ERP system that manages organizational hierarchies with departments and employees. The Composite pattern will be used to represent these hierarchical structures

    //1-Define the Component Interface:
    public interface IOrganizationComponent
    {
        string Name { get; }
        void DisplayHierarchy(int depth);
    }

    //2-Create Leaf Classes for Employees
    public class Employee : IOrganizationComponent
    {
        public string Name { get; private set; }

        public Employee(string name)
        {
            Name = name;
        }

        public void DisplayHierarchy(int depth)
        {
            Console.WriteLine(new string('-', depth) + " Employee: " + Name);
        }


    }


    //3-Create Composite Classes for Departments:
    public class Department : IOrganizationComponent
    {
        private List<IOrganizationComponent> subordinates = new List<IOrganizationComponent>();

        public string Name { get; private set; }

        public Department(string name)
        {
            Name = name;
        }

        public void AddSubordinate(IOrganizationComponent subordinate)
        {
            subordinates.Add(subordinate);
        }

        public void DisplayHierarchy(int depth)
        {
            Console.WriteLine(new string('-', depth) + " Department: " + Name);

            foreach (var subordinate in subordinates)
            {
                subordinate.DisplayHierarchy(depth + 2);
            }
        }
    }

    //4-Integrate the Composite Design Pattern into the ERP System:
    public class ERPSystem
    {
        private IOrganizationComponent rootComponent;

        public ERPSystem(IOrganizationComponent rootComponent)
        {
            this.rootComponent = rootComponent;
        }

        public void DisplayOrganizationHierarchy()
        {
            rootComponent.DisplayHierarchy(0);
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            // Create employees
            var employee1 = new Employee("John Doe");
            var employee2 = new Employee("Jane Smith");
            var employee3 = new Employee("Alice Johnson");

            // Create departments and add employees
            var development = new Department("Development");
            development.AddSubordinate(employee1);
            development.AddSubordinate(employee2);

            var marketing = new Department("Marketing");
            marketing.AddSubordinate(employee3);

            // Create more employees
            var employee4 = new Employee("Bob Williams");

            // Create a higher-level department and add departments and an employee
            var company = new Department("XYZ Corp.");
            company.AddSubordinate(development);
            company.AddSubordinate(marketing);
            company.AddSubordinate(employee4);

            // Integrate Composite Design Pattern in ERP system
            ERPSystem erpSystem = new ERPSystem(company);
            erpSystem.DisplayOrganizationHierarchy();
        }
    }

}
