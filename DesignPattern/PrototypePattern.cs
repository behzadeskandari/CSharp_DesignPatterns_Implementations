using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DesignPattern.Prototype
{
    public class PrototypePattern
    {
    }

    public interface IPrototype<T>
    {
        T DeepCopy();
    }

    public class Person : IPrototype<Person>
    {
        public string[] Names;
        public Address Address;

        public Person(Person other)
        {
            Names = other.Names;
            Address = new Address(other.Address);
        }
        public Person(string[] Names, Address Address)
        {
            Names = Names ?? throw new ArgumentNullException(paramName: nameof(Names));
            Address = Address ?? throw new ArgumentNullException(paramName: nameof(Address));
        }
        public Person DeepCopy()
        {
            return new Person(Names, Address.DeepCopy());
        }

        public override string ToString()
        {
            return $"{nameof(Names)} : {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
        }
    }

    public class Address :IPrototype<Address>
    {
        public string StreetName;
        public int HouseNumber;

        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }

        public Address(string streetName,int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public Address DeepCopy()
        {
            return new Address(StreetName, HouseNumber);
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
    }


    ////this is called type list
    ///
    //public class Person
    //{
    //    public string[] Names;
    //    public Address Address;

    //    public Person(Person other)
    //    {
    //        Names = other.Names;
    //        Address = new Address(other.Address);
    //    }
    //    public Person(string[] Names, Address Address)
    //    {
    //        Names = Names ?? throw new ArgumentNullException(paramName: nameof(Names));
    //        Address = Address ?? throw new ArgumentNullException(paramName: nameof(Address));
    //    }

    //  public override string ToString()
    //    {
    //        return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
    //    }
    //}
    //public class Address 
    //{
    //    public string StreetName;
    //    public int HouseNumber;

    //    public Address(Address other)
    //    {
    //        StreetName = other.StreetName;
    //        HouseNumber = other.HouseNumber;
    //    }

    //    public Address(string streetName, int houseNumber)
    //    {
    //        StreetName = streetName;
    //        HouseNumber = houseNumber;
    //    }
    //    public override string ToString()
    //    {
    //        return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
    //    }
    //}

    ///Deep Copy Using ICloneable
    //badPractice
    //public class PersonClone : ICloneable
    //{
    //    public string[] Names;
    //    private Address Address;

    //    public PersonClone(string[] names,Address address)
    //    {
    //        Names = Names ?? throw new ArgumentNullException(paramName: nameof(names));
    //        Address = address ?? throw new ArgumentNullException(paramName: nameof(address));
    //    }
    //    public object Clone()
    //    {
    //        return new PersonClone(Names, Address);
    //    }

    //    //  public override string ToString()
    //    //    {
    //    //        return $"{nameof(Name)}: {StreetName}, {nameof(Name)}: {HouseNumber}";
    //    //    }
    //}

    #region [Copy Throught Serilization]
    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            
            formatter.Serialize(stream, self);
            
            stream.Seek(0, SeekOrigin.Begin);
            
            object copy = formatter.Deserialize(stream);
            
            stream.Close();

            return (T)copy;
        }

        public static T DeepCopyXml<T>(this T self)
        {
            using (var ms = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(ms, self);
                ms.Position = 0;
                return (T)s.Deserialize(ms);
            }
        }

    }
    //[Serializable]
    public class Address1
    {
        public string StreetName;
        public int HouseNumber;

        public Address1()
        {

        }
        public Address1(Address1 other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }

        public Address1(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
    }
    //[Serializable]
    public class Person1
    {
        public string[] Names;
        public Address1 Address;

        public Person1(Person1 other)
        {
            Names = other.Names;
            Address = new Address1(other.Address);
        }
        public Person1(string[] Names, Address1 Address)
        {
            Names = Names ?? throw new ArgumentNullException(paramName: nameof(Names));
            Address = Address ?? throw new ArgumentNullException(paramName: nameof(Address));
        }

        public override string ToString()
        {
            return $"{nameof(Names)} : {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
        }
    }

    #endregion [Copy Throught Serilization]

    public class Program6
    {
        static void MAinMenu(string[] args)
        {
            var behzad = new Person1(new[] { "behad", "eskadari" }, new Address1("tehran iran", 10));

            //constructor copy just like C++ way
            var jane = new Person1(behzad);

            var jane2 = behzad.DeepCopy();
            var jane3 = behzad.DeepCopyXml();

            jane.Address.HouseNumber = 321;
            jane.Names[0] = "behzad";
            jane.Names[1] = "eskandari";
            
            ///gonna fial modifing the same refrence
            
            ///jane.Names[0] = "hasan";
            

            Console.WriteLine(behzad);
        }
    }

    //An Existing object (partially or fully constructed) design is a prototype
    // a partically or fully initialized object that you copy (clone) and make use of.
}
