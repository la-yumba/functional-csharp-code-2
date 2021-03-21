using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Examples.Chapter5;
using static System.Console;

namespace Examples.Chapter6
{
   public static class AskForValidAgeAndPrintFlatteringMessage
   {
      public static void Run()
         => WriteLine($"Only {ReadAge()}! That's young!");

      static Option<Age> ParseAge(string s)
         => Int.Parse(s).Bind(Age.Create);

      static Age ReadAge()
         => ParseAge(Prompt("Please enter your age")).Match(
            () => ReadAge(),
            (age) => age);      

      static string Prompt(string prompt)
      {
         WriteLine(prompt);
         return ReadLine();
      }
   }

   class SurveyOptionalAge
   {
      static IEnumerable<Subject> Population => new[]
      {
         new Subject(Age.Create(33)),
         new Subject(None), // this person did not disclose her age
         new Subject(Age.Create(37)),
      };

      internal static void _main()
      {
         var optionalAges = Population.Map(p => p.Age);
         // => [Some(33), None, Some(37)]

         var statedAges = Population.Bind(p => p.Age);
         // => [33, 37]

         var averageAge = statedAges.Average(a => a.Value);
         // => 35
      }
   }
}
