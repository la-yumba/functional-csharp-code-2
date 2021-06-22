using System;
using LaYumba.Functional;

using Name = System.String;
using Greeting = System.String;
using PersonalizedGreeting = System.String;

namespace Examples.Chapter9
{
   public class TypeInference_Method
   {
      // 1. method
      PersonalizedGreeting GreeterMethod(Greeting gr, Name name)
         => $"{gr}, {name}";

      // the below does NOT compile!
      //Func<Name, Greeting> __GreetWith(Greeting greeting)
      //   => GreeterMethod.Apply(greeting);

      // the lines below compiles, but oh my!
      Func<Name, PersonalizedGreeting> GreetWith_1(Greeting greeting)
         => FuncExt.Apply<Greeting, Name, PersonalizedGreeting>(GreeterMethod, greeting);

      Func<Name, PersonalizedGreeting> _GreetWith_2(Greeting greeting)
         => new Func<Greeting, Name, PersonalizedGreeting>(GreeterMethod)
            .Apply(greeting);
   }

   public class TypeInference_Delegate
   {
      readonly string separator = "! ";

      // 1. field
      static readonly Func<Greeting, Name, PersonalizedGreeting> GreeterField
         = (gr, name) => $"{gr}, {name}";

      Func<Name, PersonalizedGreeting> CreateGreetingWith_Field(Greeting greeting)
         => GreeterField.Apply(greeting);


      // 2. property
      Func<Greeting, Name, PersonalizedGreeting> GreeterProperty
         => (gr, name) => $"{gr}{separator}{name}";

      Func<Name, PersonalizedGreeting> CreateGreetingWith_Property(Greeting greeting)
         => GreeterProperty.Apply(greeting);


      // 3. factory
      Func<Greeting, T, PersonalizedGreeting> GreeterFactory<T>()
         => (gr, t) => $"{gr}, {t}";

      Func<Name, PersonalizedGreeting> CreateGreetingWith_Factory(Greeting greeting)
         => GreeterFactory<Name>().Apply(greeting);
   }
}