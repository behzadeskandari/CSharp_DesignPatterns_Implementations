using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class FactoryPattern
    {
    }

    #region [Inner Factor]
    //public static class PointOneFactory
    //{
    //    public static PointOne NewCartesianPoints(double x , double y)
    //    {
    //        return new PointOne(x, y);

    //    }

    //    public static PointOne NewPolarPoint(double rho , double theta)
    //    {
    //        return new PointOne(rho * Math.Cos(theta), rho * Math.Sin(theta));
    //    } 
    //}

    public class PointOne
    {
        private double x, y;
        private PointOne(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        //internal PointOne(double x, double y)
        //{
        //    this.x = x;
        //    this.y = y;
        //}

        public override string ToString()
        {
            return $"{x},{y}";
        }

        /// <summary>
        /// Return A new Object of point
        /// </summary>
        public static PointOne origin => new PointOne(0, 0);

        /// <summary>
        /// instantiate a new field
        /// </summary>
        public static PointOne Origin = new PointOne(0, 0); //its better than the returning a new object


        /// <summary>
        /// None Static Way
        /// </summary>
        //public static PointFactory Factory = new PointFactory();


        //public class PointFactory
        //{
        //    //    public static PointOne NewCartesianPoints(double x , double y)
        //    //    {
        //    //        return new PointOne(x, y);

        //    //    }

        //    //    public static PointOne NewPolarPoint(double rho , double theta)
        //    //    {
        //    //        return new PointOne(rho * Math.Cos(theta), rho * Math.Sin(theta));
        //    //    } 

        //}


        ///////////////static Way
        public static class Factory
        {
            public static PointOne NewCartesianPoints(double x, double y)
            {
                return new PointOne(x, y);

            }

            public static PointOne NewPolarPoint(double rho, double theta)
            {
                return new PointOne(rho * Math.Cos(theta), rho * Math.Sin(theta));
            }

        }
    }

    public class PR
    {
        static void MainMEthod(string[] args)
        {
            var point = PointOne.Factory.NewPolarPoint(1.0, Math.PI / 2);

            Console.WriteLine(point);
        }
    }

    #endregion [Inner Factory]


    //#region [Abstract Factory]
    //public interface IHotDrink
    //{
    //    void Consume();
    //}

    //internal class Tea : IHotDrink
    //{
    //    public int _amount { get; set; }
    //    public Tea()
    //    {

    //    }

    //    public Tea(int amount)
    //    {
    //        _amount = amount;
    //    }


    //    public void Consume()
    //    {
    //        Console.WriteLine($"this tea is nice but i prefer it with milk british style");

    //        Console.WriteLine($"The Amount of Tea Needed is {_amount}");
    //    }
    //}
    //internal class Coffe : IHotDrink
    //{
    //    public void Consume()
    //    {
    //        Console.WriteLine($"this coffe is nice");
    //    }
    //}

    //public interface IHotDrinkFactory
    //{
    //    IHotDrink Prepare(int amount);
    //}
    //internal class TeaFactory : IHotDrinkFactory
    //{
    //    public IHotDrink Prepare(int amount)
    //    {
    //        Console.WriteLine($"the Amount of coffe needed is {amount}");
    //        return new Tea();
    //    }
    //}
    //internal class CoffeFactory : IHotDrinkFactory
    //{
    //    public IHotDrink Prepare(int amount)
    //    {
    //        Console.WriteLine($"the Amount of coffe needed is {amount}");
    //        return new Coffe();
    //    }
    //}

    //public class HotDrinkMachine
    //{
    //    public enum AvailableDrink
    //    {
    //        Coffe,Tea
    //    }

    //    private Dictionary<AvailableDrink, IHotDrinkFactory> factories = new Dictionary<AvailableDrink, IHotDrinkFactory>();
    //    public HotDrinkMachine()
    //    {
    //        foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
    //        {
    //            var factory = (IHotDrinkFactory)Activator.CreateInstance(Type.GetType("DesignPattern." + Enum.GetName(typeof(AvailableDrink) , drink) + "Factory"));

    //            factories.Add(drink, factory);
    //        }
    //    }

    //    public IHotDrink MakeDrink(AvailableDrink drink,int amount)
    //    {
    //        return factories[drink].Prepare(amount);
    //    }

    //}

    //public class PR6
    //{
    //    static void Main(string[] args)
    //    {
    //        //var TeaFav = new TeaFactory();
    //        //var tea = TeaFav.Prepare(0);
    //        //tea.Consume();

    //        var machine = new HotDrinkMachine();
    //        var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Tea, 1000);
    //        drink.Consume();

    //    }
    //}

    //#endregion [Abstract Factory]




    #region [Abstract Factory And OCP]
    public interface IHotDrink
    {
        void Consume();
    }

    internal class Tea : IHotDrink
    {
        public int _amount { get; set; }
        public Tea()
        {

        }

        public Tea(int amount)
        {
            _amount = amount;
        }


        public void Consume()
        {
            Console.WriteLine($"this tea is nice but i prefer it with milk british style");

            Console.WriteLine($"The Amount of Tea Needed is {_amount}");
        }
    }
    internal class Coffe : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine($"this coffe is nice");
        }
    }

    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }
    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"the Amount of coffe needed is {amount}");
            return new Tea();
        }
    }
    internal class CoffeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"the Amount of coffe needed is {amount}");
            return new Coffe();
        }
    }

    public class HotDrinkMachine
    {
        private List<Tuple<string, IHotDrinkFactory>> factories = new List<Tuple<string, IHotDrinkFactory>>();

        public HotDrinkMachine()
        {
            foreach (var typ in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                if (typeof(IHotDrinkFactory).IsAssignableFrom(typ) && !typ.IsInterface)
                {
                    factories.Add(Tuple.Create(
                        typ.Name.Replace("Factory", String.Empty),
                        (IHotDrinkFactory)Activator.CreateInstance(typ)
                        ));
                }
            }
        }

        public IHotDrink MakeDrink()
        {
            Console.WriteLine("Available Drinks");

            for (int index = 0; index < factories.Count; index++)
            {
                var tuple = factories[index];
                Console.WriteLine($"{index}: {tuple.Item1}");
            }
            while (true)
            {
                string s;
                if ((s = Console.ReadLine()) != null && int.TryParse(s,out int i) && i >= 0 && i < factories.Count)
                {
                    Console.WriteLine("Specify Amount");
                    s = Console.ReadLine();
                    if (s != null && int.TryParse(s,out int amount) && amount > 0)
                    {
                        return factories[i].Item2.Prepare(amount);
                    }
                }
                Console.WriteLine("Incorrect Input Try Again");
            }
        }
    }

    public class Pr7
    {
        static void MainM(string[] args)
        {
            var machine = new HotDrinkMachine();
            var drink = machine.MakeDrink();
            drink.Consume();

        }
    }
    #endregion [Abstract Factory And OCP]

}
