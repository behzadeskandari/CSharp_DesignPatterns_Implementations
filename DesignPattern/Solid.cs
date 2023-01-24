using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class Solid
    {
    }

    #region [Single Responsibility]

    public class Journal
    {
        private readonly List<string> entries = new List<string>();

        private static int Count = 0;

        public int AddEntry(string text)
        {
            entries.Add($"{++Count} : {text}");
            return Count;
        }

        public void RemoveEntries(int index)
        {
            entries.RemoveAt(index);
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, entries);
        }
    }

    public class Demo
    {
        static void MainMenu2(string[] args)
        {
            var j = new Journal();
            j.AddEntry("I Had A Good Day YesterDay");
            j.AddEntry("I ate a burger");
            Persitance persitance = new Persitance();
            persitance.SaveToFile(j, "behzadeskadnri", true);
            Console.WriteLine(j);

        }
    }

    public class Persitance
    {
        internal void SaveToFile(Journal journal, string filename, bool overwrite = false)
        {
            if (overwrite || File.Exists(filename))
            {
                File.WriteAllText(filename, ToString());
            }
        }
    }

    #endregion [Single Responsibility]

    #region [OpenClosePrinciple]
    public enum Color
    {
        Red, Green, Blue,
    }

    public enum Size
    {
        Small, Medium, Large, Huge
    }
    public class Product
    {
        public string Name;
        public Color Color;
        public Size Size;

        public Product(string name, Color color, Size size)
        {
            Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
            Color = color;
            Size = size;
        }

    }

    public class ProductFilter
    {
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
        {
            foreach (var item in products)
            {
                if (item.Size == size)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
        {
            foreach (var item in products)
            {
                if (item.Color == color)
                {
                    yield return item;
                }
            }
        }
        ///Breaks the open close principle we have to extends the ocp
        //public IEnumerable<Product> FilterByColor(IEnumerable<Product> products,Color color,Size size)
        //{
        //    foreach (var item in products)
        //    {
        //        if (item.Color == color)
        //        {
        //            yield return item;
        //        }
        //    }
        //}

    }


    public interface ISpecification<T>
    {
        bool IsSatisfied(T type);
    }

    public interface IFilter<T>
    {
        IEnumerable<T> Filters(IEnumerable<T> items, ISpecification<T> spec);
    }

    public class ColorSpefication : ISpecification<Product>
    {
        private Color Color;
        public ColorSpefication(Color color)
        {
            Color = color;
        }

        public bool IsSatisfied(Product type)
        {
            return type.Color == Color;

        }


    }

    public class SizeSpecification : ISpecification<Product>
    {
        private Size Size;

        public SizeSpecification(Size size)
        {
            Size = size;
        }

        public bool IsSatisfied(Product type)
        {
            return type.Size == Size;
        }
    }

    public class AndSpecificaiton<T> : ISpecification<T>
    {
        ISpecification<T> first, second;

        public AndSpecificaiton(ISpecification<T> first, ISpecification<T> second)
        {
            this.first = first ?? throw new ArgumentNullException(paramName: nameof(first));
            this.second = second ?? throw new ArgumentNullException(paramName: nameof(second));
        }

        public bool IsSatisfied(T type)
        {
            return first.IsSatisfied(type) & second.IsSatisfied(type);
        }
    }

    public class BetterFilter : IFilter<Product>
    {

        public IEnumerable<Product> Filters(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var item in items)
            {
                if (spec.IsSatisfied(item))
                {
                    yield return item;
                }
            }
        }
    }

    public class Demo2
    {
        static void MainMethodTow(string[] args)
        {
            var apple = new Product("apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var House = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, House };
            var pf = new ProductFilter();

            Console.WriteLine("Green Product (old):");

            foreach (var p in pf.FilterByColor(products, Color.Green))
            {
                Console.WriteLine($"- {p.Name} is Green");
            }
            var bf = new BetterFilter();
            Console.WriteLine("Green Product (new): ");
            foreach (var p in bf.Filters(products, new ColorSpefication(Color.Green)))
            {
                Console.WriteLine($" - {p.Name} is Green ");
            }

            Console.WriteLine("Large Blue Item (old) :");

            foreach (var item in bf.Filters(products, new AndSpecificaiton<Product>(
                new ColorSpefication(Color.Blue),
                new SizeSpecification(Size.Large)
                )))
            {
                Console.WriteLine($"{item.Name} is big And Blue");
            }
        }
    }
    #endregion [OpenClosePrinciple]

    #region [Liskov Subsitution Principle]
    public class Rectangle
    {
        //dont do this 
        //public int Width {get;set;}
        //public int Height { get; set; }

        public virtual int Width { get; set; }
        public virtual int Hight { get; set; }

        public Rectangle()
        {

        }

        public Rectangle(int width, int height)
        {
            Width = width;
            Hight = height;
        }

        public override string ToString()
        {
            return $"{nameof(Width)} : {Width} , {nameof(Hight)}: {Hight}";
        }

    }

    public class Square : Rectangle
    {
        //dont do this
        //public new int Width
        //{
        //    set { base.Width = base.Hight = value; }
        //}
        //public new int Hight
        //{
        //    set { base.Width = base.Hight = value; }
        //}

        ///do this inetead 
        public override int Hight { set { base.Width = base.Hight = value; } }
        public override int Width { set { base.Width = base.Hight = value; } }
    }

    public class Demo3
    {
        static public int Area(Rectangle r) => r.Width * r.Hight;

        static void MainMethodThree(string[] args)
        {
            Rectangle rc = new Rectangle(2, 6);

            Area(rc);

            Console.WriteLine($"{rc} has a area {Area(rc)}");


            //works
            Square sqtest = new Square();


            ///wont work
            Rectangle sq = new Square();
            Area(rc);
            Console.WriteLine($"{sq} has a Area {Area(sq)}");
        }
    }

    #endregion [Liskov Subsitution Principle]

    #region [Interface Segregation Principle]

    public class Document
    {

    }

    public interface IMachine
    {
        void Print(Document d);
        void Scan(Document d);
        void Fax(Document d);
    }

    public class Machine : IMachine
    {
        public void Fax(Document d)
        {
            throw new NotImplementedException();
        }

        public void Print(Document d)
        {
            throw new NotImplementedException();
        }

        public void Scan(Document d)
        {
            throw new NotImplementedException();
        }
    }


    public class OldFashiondPrinter : IMachine
    {
        public void Fax(Document d)
        {
            ///just this one needs
            throw new NotImplementedException();
        }

        public void Print(Document d)
        {
            throw new NotImplementedException();
        }

        public void Scan(Document d)
        {
            throw new NotImplementedException();
        }
    }


    public interface IPrinter
    {
        void Print(Document d);
    }
    public interface IScanner
    {
        void Scan(Document d);
    }
    public interface IFaxer
    {
        void Fax(Document d);
    }

    public class OldFashiondPrinterSecond : IPrinter
    {
        public void Print(Document d)
        {
            throw new NotImplementedException();
        }
    }


    public class MachineSecond : IPrinter, IScanner, IFaxer
    {
        public void Fax(Document d)
        {
            throw new NotImplementedException();
        }

        public void Print(Document d)
        {
            throw new NotImplementedException();
        }

        public void Scan(Document d)
        {
            throw new NotImplementedException();
        }

    }

    public interface IMultiFunctionMachine : IPrinter, IScanner, IFaxer
    {

    }
    public class MultiFunctionMachine : IMultiFunctionMachine
    {
        private IPrinter printer;
        private IScanner scanner;
        private IFaxer faxer;

        public MultiFunctionMachine(IPrinter printer, IScanner scanner, IFaxer faxer)
        {
            this.printer = printer ?? throw new ArgumentNullException(paramName: nameof(printer));
            this.scanner = scanner ?? throw new ArgumentNullException(paramName: nameof(scanner));
            this.faxer = faxer ?? throw new ArgumentNullException(paramName: nameof(faxer));
        }

        public void Fax(Document d)
        {
            throw new NotImplementedException();
        }

        public void Print(Document d)
        {
            throw new NotImplementedException();
        }

        public void Scan(Document d)
        {
            throw new NotImplementedException();
        }
    }

    #endregion [Interface Segregation Principle]


    #region [Dependency Injection]
    public enum Relationship
    {
        Parent, Child, Sibling
    }

    public class Person2
    {
        public string Name;
    }


    public interface IRelationShipBrowser
    {
        IEnumerable<Person2> FindAllChildren(string name);
    }


    public class RelationshipsCls : IRelationShipBrowser
    {

        //not a good practice to set the list public set it private 
        public static List<(Person2, Relationship, Person2)> relations = new List<(Person2, Relationship, Person2)>();
        public void AddParentAndChild(Person2 parent, Person2 child)
        {
            relations.Add((parent, Relationship.Parent, child));
            relations.Add((child, Relationship.Child, parent));
        }
        
        public IEnumerable<Person2> FindAllChildren(string name)
        {
            return relations.Where(x => x.Item1.Name == name && x.Item2 == Relationship.Parent).Select(r => r.Item3);
        
        }

        
    }

    public class Research
    {
        

        //not a good practice
        public Research(RelationshipsCls relationship)
        {
            var relations = relationship.relations;
            foreach (var item in relations.Where(x => x.Item1.Name == "behzad" && x.Item2 == Relationship.Parent))
            {
                Console.WriteLine($"behad has a child called {item.Item3.Name}");
            }
        }
        public Research(IRelationShipBrowser browser)
        {
            foreach (var p in browser.FindAllChildren("behzad"))
            {
                Console.WriteLine($"behzad has a child called {p.Name}");
            }
        }

        static void MainMehtod(string[] args)
        {
            var parent = new Person2() { Name = "behzad" };
            var child1 = new Person2() { Name = "Majid" };
            var child2 = new Person2() { Name = "Hasan" };

            var relationship = new RelationshipsCls();
            relationship.AddParentAndChild(parent: parent, child: child1);
            relationship.AddParentAndChild(parent: parent, child: child2);
            
            //new Research(relationship);
        
        }
    }
    #endregion [Dependency Injection]
}
