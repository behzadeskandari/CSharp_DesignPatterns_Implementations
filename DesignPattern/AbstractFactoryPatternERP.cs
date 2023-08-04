using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class AbstractFactoryPatternERP
    {
    }

    /// <summary>
    /// The Abstract Factory Pattern allows us to group related object (reports) into families (reportTypes) and create them without specifying their concrete classes this promotes the 
    /// flexibility to extend the ERP system with new report types in the future without affecting the client code 
    /// </summary>

    public interface IReport
    {
        void Generaate();
    }


    public class SalesReport : IReport
    {
        public void Generaate()
        {
            Console.WriteLine("generate Sales Report");
        }
    }
    public class InventoryReport : IReport
    {
        public void Generaate()
        {
            Console.WriteLine("generate Sales Report");
        }
    }

    public interface IReportFactory
    {
        IReport CreateReport();
    }


    public class SalesReportFactory : IReportFactory
    {
        public IReport CreateReport()
        {
            return new SalesReport();
        }
    }


    public class InventoryReportFactory: IReportFactory
    {
        public IReport CreateReport()
        {
            return new InventoryReport();
        }
    }


    class ProgramPattern
    {
        static void Main()
        {
            GenerateReport(new SalesReportFactory());
            GenerateReport(new InventoryReportFactory());
        }

        public static void GenerateReport(IReportFactory reportFactory)
        {
            IReport report = reportFactory.CreateReport();
            report.Generaate();
        }
    }

}
