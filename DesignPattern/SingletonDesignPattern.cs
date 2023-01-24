using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class SingletonDesignPattern
    {
    }

    #region [Singleton DesignPattern]
    public interface IDatabase
    {
        int GetPopulation(string name);
    }

    public class SignletonDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;
        private static int instanceCount;
        public static int Count => instanceCount;

        public SignletonDatabase()
        {
            instanceCount++;
            Console.WriteLine("Initilazing the capitals");

            capitals = File.ReadAllLines("Capital.txt").Batch(2).ToDictionary(list => list.ElementAt(0).Trim(),list=> int.Parse(list.ElementAt(1)));
        }
        public int GetPopulation(string name)
        {
            return capitals[name];
        }

        private static Lazy<SignletonDatabase> instance = new Lazy<SignletonDatabase>(() => new SignletonDatabase());

        public static SignletonDatabase Instance = instance.Value;

    }

    /// <summary>
    /// Better Way
    /// </summary>
    public class ConfigurableRecordFinder
    {
        private IDatabase _database;
        public ConfigurableRecordFinder(IDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(paramName: nameof(database));
        }
    }



    #endregion [Singleton DesignPattern]
}
