using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class GenericValueAdapterPattern
    {
    }

    ///we use this pattern to adapt existing classes with diffrent interfaces and the Generic value adapter pattern to convert SUpplier-specific product data into a common format that the ERP system Understand



    ///step 1 - define the common product interface 
    public interface IEmployee
    {
        string Name { get; }
        decimal Salary { get; }
        // Other common employee properties and methods...
    }

    public class Employee
    {
        public string EmployeeName { get; set; }
        public decimal BaseSalary { get; set; }
        // Other employee-specific properties...
    }

    public class Manager
    {
        public string ManagerName { get; set; }
        public decimal BaseSalary { get; set; }
        public int NumberOfSubordinates { get; set; }
        // Other manager-specific properties...
    }

    public class EmployeeAdapter<TEmployee> : IEmployee
    where TEmployee : new()
    {
        private TEmployee employee;

        public EmployeeAdapter(TEmployee employee)
        {
            this.employee = employee;
        }

        public string Name
        {
            get
            {
                if (typeof(TEmployee) == typeof(Employee))
                {
                    return ((Employee)(object)employee).EmployeeName;
                }
                else if (typeof(TEmployee) == typeof(Manager))
                {
                    return ((Manager)(object)employee).ManagerName;
                }

                throw new NotSupportedException("Employee type not supported.");
            }
        }

        public decimal Salary
        {
            get
            {
                if (typeof(TEmployee) == typeof(Employee))
                {
                    return ((Employee)(object)employee).BaseSalary;
                }
                else if (typeof(TEmployee) == typeof(Manager))
                {
                    return ((Manager)(object)employee).BaseSalary;
                }

                throw new NotSupportedException("Employee type not supported.");
            }
        }

        // Implement other common employee properties and methods...
    }



    public class ERPSystem
    {
        private List<IEmployee> employees;

        public ERPSystem()
        {
            employees = new List<IEmployee>();
        }

        public void AddEmployee(IEmployee employee)
        {
            employees.Add(employee);
        }

        public void DisplayEmployees()
        {
            foreach (var employee in employees)
            {
                Console.WriteLine($"Employee Name: {employee.Name}, Salary: {employee.Salary}");
            }
        }
    }

    class Program45
    {
        public static void Main(string[] args)
        {
            var employee = new Employee { EmployeeName = "John Doe", BaseSalary = 50000 };
            var manager = new Manager { ManagerName = "Jane Smith", BaseSalary = 75000, NumberOfSubordinates = 5 };

            var employeeAdapter = new EmployeeAdapter<Employee>(employee);
            var managerAdapter = new EmployeeAdapter<Manager>(manager);

            var erpSystem = new ERPSystem();
            erpSystem.AddEmployee(employeeAdapter);
            erpSystem.AddEmployee(managerAdapter);

            erpSystem.DisplayEmployees();
        }
    }
}
