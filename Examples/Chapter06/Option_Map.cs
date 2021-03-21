using System;
using static System.Console;

using LaYumba.Functional;
using static LaYumba.Functional.F;

using NUnit.Framework;

using Age = Examples.Chapter5.Age;

namespace Examples
{
   public enum Gender { Female, Male };
}

namespace Examples.Chapter6
{
   class Option_Map_Example
   {
      [Test]
      internal static void BasicExample()
      {
         Func<string, string> greet = name => $"hello, {name}";

         Option<string> empty = None;
         Option<string> optJohn = Some("John");

         Assert.AreEqual(None, empty.Map(greet));
         Assert.AreEqual(Some("hello, John"), optJohn.Map(greet));
      }

      Option<Risk> RiskOf(Subject subject)
         => subject.Age.Map(CalculateRiskProfile);

      public static Risk CalculateRiskProfile(Age age)
         => (age < 60) ? Risk.Low : Risk.Medium;
   }

   record Subject
   (
      Option<Age> Age
      // many more fields...
   );

   class Person
   {
      public string Name;
      public Option<Relationship> Relationship;      
   }

   class Relationship
   {
      public string Type;
      public Person Partner;
   }

   static class Option_Match_Example2
   {
      internal static void _main()
      {
         Person grace = new Person { Name = "Grace" }
            , dimitry = new Person { Name = "Dimitry" }
            , armin = new Person { Name = "Armin" };

         grace.Relationship = new Relationship {
            Type = "going out with", Partner = dimitry };

         WriteLine(grace.RelationshipStatus());
         // prints: Grace is going out with Dimitry

         WriteLine(armin.RelationshipStatus());
         // prints: Armin is single

         ReadKey();
      }

      static string RelationshipStatus(this Person p)
         => p.Relationship.Match(
            Some: r => $"{p.Name} is {r.Type} {r.Partner.Name}",
            None: () => $"{p.Name} is single");
   }
}
