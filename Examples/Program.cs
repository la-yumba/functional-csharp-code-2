using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using LaYumba.Functional;

// workaround to enable C# 9 syntax
namespace System.Runtime.CompilerServices { public class IsExternalInit { } }

namespace Examples
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var cliExamples = new Dictionary<string, Action>
         {
            ["HOFs"] = Chapter1.HOFs.Run,
            ["Greetings"] = Chapter7.Greetings.Run,
            ["Timer"] = Chapter14.CreatingObservables.Timer.Run,
            ["Subjects"] = Chapter14.CreatingObservables.Subjects.Run,
            ["Create"] = Chapter14.CreatingObservables.Create.Run,
            ["Generate"] = Chapter14.CreatingObservables.Generate.Run,
            ["CurrencyLookup_Unsafe"] = Chapter14.CurrencyLookup_Unsafe.Run,
            ["CurrencyLookup_Safe"] = Chapter14.CurrencyLookup_Safe.Run,
            ["VoidContinuations"] = Chapter14.VoidContinuations.Run,
            ["KeySequences"] = Chapter14.KeySequences.Run,
         };

         if (args.Length > 0)
            cliExamples.Lookup(args[0])
               .Match(
                  None: () => Console.WriteLine($"Unknown option: '{args[0]}'"),
                  Some: (main) => main()
               );

         else StartWebApi();
      }

      static void StartWebApi()
         => Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .Build()
            .Run();
   }
}
