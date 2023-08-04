using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern {
    #region [Builder]
    public class HtmlElement
    {
        public string Name, Text;

        public List<HtmlElement> Elements = new List<HtmlElement>();

        private const int indentSize = 2;
        public HtmlElement()
        {

        }

        public HtmlElement(string name, string text)
        {
            Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
            Text = text ?? throw new ArgumentNullException(paramName: nameof(text)); ;
        }

        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indent);
            sb.AppendLine($"{i}<{Name}>");
            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', indentSize * (indent + 1)));
                sb.Append(Text);

            }

            foreach (var e in Elements)
            {

                sb.Append(e.ToStringImpl(indent + 1));
            }
            sb.AppendLine($"{i} </{Name}>");
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }

    }

    public class HtmlBuilder
    {
        private readonly string _rootName;

        HtmlElement root = new HtmlElement();
        public HtmlBuilder(string rootName)
        {
            root.Name = rootName;
            _rootName = rootName;

        }
        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new HtmlElement { Name = _rootName };
        }
    }

    public class Demo
    {
        public void MainMethod1(string[] args)
        {
            //var hello = "hello";
            //var sb = new StringBuilder();
            //sb.Append("<p>");
            //sb.Append(hello);
            //sb.Append("</p>");

            //Console.WriteLine(sb);

            //var words = new[] { "hello", "world" };
            //sb.Clear();
            //sb.Append("<ul>");
            //foreach (var word in words)
            //{
            //    sb.AppendFormat("<li>{0}</li>", word);

            //}

            //sb.Append("</ul>");
            //Console.WriteLine(sb);

            var builder = new HtmlBuilder("ul");
            builder.AddChild("li", "hello").AddChild("li", "world").Clear();
            Console.WriteLine(builder.ToString());
        }
    }
    #endregion [Builder]
}


namespace DesignPattern
{

    #region [Fluent Builder inheritance With Recursive Generics]

    //is for getting fluent code inherite
    //Instead of this we can Use a Sub class Name Builder
    //public class PersonOne
    //{
    //    public string Name { get; set; }
    //    public string Position { get; set; }

    //    public override string ToString()
    //    {
    //        return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
    //    }
    //}

    //public class PersonInfoBuilder
    //{
    //    protected PersonOne person = new PersonOne();
    //    public PersonInfoBuilder Callled(string name)
    //    {
    //        person.Name = name;
    //        return this;
    //    }

    //}

    //public class PersonJobBuilder : PersonInfoBuilder
    //{
    //    public PersonJobBuilder WorksAs(string position)
    //    {
    //        person.Position = position;
    //        return this;
    //    }
    //}
    /// <summary>
    /// STEP 1 Build THe Class
    /// </summary>
    //public class PersonOne
    //{
    //    public string Name { get; set; }
    //    public string Position { get; set; }

    //    public override string ToString()
    //    {
    //        return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
    //    }
    //    /// <summary>
    //    /// Step 5 class Exposing its own bulder RECURSIVE GENERIC Approch
    //    /// </summary>
    //    public class Builder : PersonJobBuilder<Builder>
    //    {

    //    }
    //    /// <summary>
    //    /// step 6 class Exposing its own builder
    //    /// </summary>
    //    public static Builder New = new Builder();

    //}

    ///// <summary>
    ///// STEP 2 Build Abstract class
    ///// </summary>
    //public abstract class PersonBuilder
    //{
    //    protected PersonOne person = new PersonOne();
    //    public PersonOne Build()
    //    {
    //        return person;
    //    }
    //}
    ////NOte
    /////we need some kind of restriction we want to restric the self to the inherithance of personBuilder of Self
    /////you have argument and you expect the argument inherite from the current object 
    /////how would this possibly be 
    /////well this is only situation we allowing SELF refring to the object inheriting from this object 
    /////class Foo : Bar<Foo>

    ///// <summary>
    ///// STEP 3 Build Generic class that inherit from the abstract class with constraint Check on the generic Parameter
    ///// </summary>

    //public class PersonInfoBuilder<SELF> : PersonBuilder where SELF : PersonInfoBuilder<SELF>
    //{
    //    public SELF Called(string name)
    //    {
    //        person.Name = name;
    //        return (SELF)this;
    //    }
    //}


    ////this is called type list
    ///// <summary>
    ///// Step 4 Build second generic class builder that inherit from the first builder with generic constraint
    ///// </summary>
    ///// <typeparam name="SELF"></typeparam>

    //public class PersonJobBuilder<SELF> : PersonInfoBuilder<PersonJobBuilder<SELF>> where SELF : PersonJobBuilder<SELF>
    //{
    //    public SELF WorkAsA(string position)
    //    {
    //        person.Position = position;
    //        return (SELF)this;
    //    }
    //}


    //internal class ProgramThree
    //{
    //    public static void MainMethod(string[] args)
    //    {
    //        //wont work 
    //        //var builder = new PersonJobBuilder();

    //        var person = new PersonOne.Builder();
    //        var me = PersonOne.New.Called("behzad").WorkAsA("defense");
    //        Console.WriteLine(me);

    //    }
    //}
    #endregion [Fluent Builder inheritance With Recursive Generics]

    #region [Funtional Programming BUilder]
    public class PersonTwo
    {
        public string Name, Position;
    }

    /////////////////////////////////////////////////////////First Approach
    /// <summary>
    /// 
    /// </summary>
    //public sealed class PersonBuilderFunctional
    //{
    //    private readonly List<Func<PersonTwo, PersonTwo>> actions = new List<Func<PersonTwo, PersonTwo>>();
    //    public PersonBuilderFunctional Called(string name) => Do(p => p.Name = name);

    //    public PersonTwo Build() => actions.Aggregate(new PersonTwo(), (pair, function) => function(pair));
    //    private PersonBuilderFunctional AddAction(Action<PersonTwo> action)
    //    {
    //        actions.Add(p =>
    //        {
    //            action(p);
    //            return p;
    //        });
    //        return this;
    //    }
    //    public PersonBuilderFunctional Do(Action<PersonTwo> action) => AddAction(action);

    //}


    //public static class PersonBuilderExtension
    //{
    //    public static PersonBuilderFunctional WorkAs(this PersonBuilderFunctional builder,string position)
    //    {
    //        return builder.Do(p => p.Position = position);
    //    }
    //}
    //////////////////////////////////////////////End oF First Approach

    /////////////////////////////////////////////////////////////////Generic Functional Builder
    //public abstract class FunctionalBuilder<TSubject, TSelf> where TSelf : FunctionalBuilder<TSubject, TSelf> where TSubject : new()
    //{
    //    private readonly List<Func<PersonTwo, PersonTwo>> actions = new List<Func<PersonTwo, PersonTwo>>();

    //    public TSelf Called(string name) => Do(p => p.Name = name);

    //    public PersonTwo Build() => actions.Aggregate(new PersonTwo(), (pair, function) => function(pair));

    //    private TSelf AddAction(Action<PersonTwo> action)
    //    {
    //        actions.Add(p =>
    //        {
    //            action(p);
    //            return p;
    //        });
    //        return (TSelf)this;
    //    }
    //    public TSelf Do(Action<PersonTwo> action) => AddAction(action);
    //}

    //public sealed class PersonBuilderFunctional : FunctionalBuilder<PersonTwo, PersonBuilderFunctional>
    //{
    //    public PersonBuilderFunctional DoSomething(string name)
    //    {
    //        return Do(p => p.Name = name);
    //    }
    //}
    ///////////////////////////////////////////////////////////////////END oF Generic Functional Builder
    //internal class Program3
    //{
    //    public static void MainM(string[] args)
    //    {
    //        var perosn = new PersonBuilderFunctional().Called("behzad").DoSomething("Developer").Build();
    //    }
    //}

    #endregion

    #region [Faceted Builder]
    //public class Person
    //{
    //    public string StreetAddress, PostCode, City;
    //    public string CompanyName, Position;
    //    public int annualIncome;

    //    public override string ToString()
    //    {
    //        return $"{nameof(StreetAddress)}: {StreetAddress}, {nameof(PostCode)}: {PostCode}, {nameof(City)}: {City}, {nameof(CompanyName)}: {CompanyName}, {nameof(Position)}: {Position}, {nameof(annualIncome)}: {annualIncome}";

    //    }

    //}

    //public static class PersonCreator
    //{
    //    public static Person CreatePerson()
    //    {
    //       return new Person();
    //    }
    //}
    //public class PersonBuilder1 //facade
    //{
        

    //    protected Person person = new Person();

    //    public PersonJobBuilder Works = new PersonJobBuilder(PersonCreator.CreatePerson());

    //    public PersonAddressBuilder Lives => new PersonAddressBuilder(person);


    //    public static implicit operator Person(PersonBuilder1 pb)
    //    {
    //        return pb.person; 
    //    }
    //}



    //public class PersonJobBuilder : PersonBuilder1
    //{
    //    public PersonJobBuilder(Person person)
    //    {
    //        this.person = person;
    //    }

    //    public PersonJobBuilder At(string companyname)
    //    {
    //        person.CompanyName = companyname;
    //        return this;
    //    }
    //    public PersonJobBuilder AsA(string position)
    //    {
    //        person.Position = position;
    //        return this;
    //    }
    //    public PersonJobBuilder Earning(int amount)
    //    {
    //        person.annualIncome = amount;
    //        return this;
    //    }

    //}


    //public class PersonAddressBuilder : PersonBuilder1
    //{
    //    public PersonAddressBuilder(Person person)
    //    {
    //        this.person = person;
    //    }
    //    public PersonAddressBuilder Address(string address)
    //    {
    //        person.StreetAddress = address;
    //        return this;
    //    }
    //    public PersonAddressBuilder PostCode(string postcode)
    //    {
    //        person.PostCode = postcode;
    //        return this;
    //    }
    //    public PersonAddressBuilder City(string city)
    //    {
    //        person.City = city;
    //        return this;
    //    }
    //}

    //class ProgramBuilder
    //{
    //    public static void MAinMethod4(string[] args)
    //    {
    //        var pb = new PersonBuilder1();

    //        Person perosn = pb.Works.At("Factor").AsA("Worker").Earning(12000);
    //        Person personDeails = pb.Lives.City("Tehran").Address("Gisha ave 29").PostCode("012WW12211");
    //    }
    //}

    
    #endregion


}


