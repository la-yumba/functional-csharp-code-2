using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using LaYumba.Functional;

using Boc.Services;
using Boc.Commands;
using System.Threading.Tasks;

// workaround to enable C# 9 syntax
namespace System.Runtime.CompilerServices { public class IsExternalInit { } }

namespace Examples
{
   public class Program
   {
      public async static Task Main(string[] args)
      {
         var cliExamples = new Dictionary<string, Action>
         {
            ["HOFs"] = Chapter2.HOFs.Run,
            ["NaivePar"] = Chapter3.ListFormatter.Parallel.Naive.ListFormatter.Run,
            ["OptionBind"] = Chapter6.AskForValidAgeAndPrintFlatteringMessage.Run,
            ["Greetings"] = Chapter9.Greetings.Run,
            ["Timer"] = Chapter16.CreatingObservables.Timer.Run,
            ["Subjects"] = Chapter16.CreatingObservables.Subjects.Run,
            ["Create"] = Chapter16.CreatingObservables.Create.Run,
            ["Generate"] = Chapter16.CreatingObservables.Generate.Run,
            ["CurrencyLookup_Unsafe"] = Chapter16.CurrencyLookup_Unsafe.Run,
            ["CurrencyLookup_Safe"] = Chapter16.CurrencyLookup_Safe.Run,
            ["VoidContinuations"] = Chapter16.VoidContinuations.Run,
            ["KeySequences"] = Chapter16.KeySequences.Run,
         };

         if (args.Length > 0)
            cliExamples.Lookup(args[0])
               .Match(
                  None: () => Console.WriteLine($"Unknown option: '{args[0]}'"),
                  Some: (main) => main()
               );

         else
            await Boc.Chapter9.Program.Run();
            //StartWebApi();
      }

      static void StartWebApi()
         => Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
               services.AddControllers();
               services.AddSwaggerGen();

               // Chapter 3
               // inject an interface
               services.AddTransient<Chapter03.Boc.InjectInterface.IDateTimeService, Chapter03.Boc.InjectInterface.DefaultDateTimeService>();
               services.AddTransient<IValidator<MakeTransfer>, Chapter03.Boc.InjectInterface.DateNotPastValidator_Record>();

               // inject a value
               services.AddTransient<IValidator<MakeTransfer>, Chapter03.Boc.InjectValue.DateNotPastValidator>
                  (_ => new Chapter03.Boc.InjectValue.DateNotPastValidator(DateTime.UtcNow.Date));

               // inject a func
               services.AddTransient<IValidator<MakeTransfer>, Chapter03.Boc.InjectFunc.DateNotPastValidator>
                  (_ => new Chapter03.Boc.InjectFunc.DateNotPastValidator(() => DateTime.UtcNow.Date));

               // inject a delegate
               services.AddTransient<Chapter03.Boc.InjectDelegate.Clock>(_ => () => DateTime.UtcNow);
               services.AddTransient<IValidator<MakeTransfer>, Chapter03.Boc.InjectDelegate.DateNotPastValidator>();
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.Configure(app =>
            {
               app.UseDeveloperExceptionPage()
                  .UseSwagger()
                  .UseSwaggerUI(swagger =>
                  {
                     swagger.SwaggerEndpoint("v1/swagger.json", "Examples API");
                     swagger.RoutePrefix = string.Empty;
                  })
                  .UseRouting()
                  .UseEndpoints(endpoints => endpoints.MapControllers());
            }))
            .Build()
            .Run();
   }
}
