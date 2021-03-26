using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Boc.Commands;
using Microsoft.Extensions.Configuration;
using Examples;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using Unit = System.ValueTuple;
using System.Collections.Generic;
using Boc.Domain;

namespace Boc.Chapter9
{
   using static ActionResultFactory;

   public static class Program
   {
      public static WebApplication MapPost<T>
         (this WebApplication app, string route, Func<T, ActionResult> handle)
      {
         app.MapPost(route, async http =>
         {
            T t = await http.Request.ReadFromJsonAsync<T>();

            ActionResult result = handle(t);

            http.Response.StatusCode = result.StatusCode;

            switch (result)
            {
               case OkObjectResult r:
                  { await http.Response.WriteAsJsonAsync(r.Value); break; }
               case BadRequestObjectResult r:
                  { await http.Response.WriteAsJsonAsync(r.Errors); break; }
            };
         });

         return app;
      }

      public async static Task Run()
      {
         var app = WebApplication.Create();
         Func<MakeTransfer, ActionResult> handleSaveTransfer = ConfigureSaveTransferHandler(app.Configuration);

         app.MapPost("/Transfer/Future", handleSaveTransfer);

         await app.RunAsync();
      }

      static Func<MakeTransfer, ActionResult>
         ConfigureSaveTransferHandler(IConfiguration config)
      {
         // persistence layer
         ConnectionString connString = config.GetSection("ConnectionString").Value;
         SqlTemplate InsertTransferSql = "INSERT ...";

         var save = connString.TryExecute(InsertTransferSql);

         var validate = Validation.DateNotPast(clock: () => DateTime.UtcNow);

         return HandleSaveTransfer(validate, save);
      }

      static Func<MakeTransfer, ActionResult> HandleSaveTransfer
         ( Validator<MakeTransfer> validate
         , Func<MakeTransfer, Exceptional<Unit>> save)
         => transfer
         => validate(transfer).Map(save).Match
            (
               Invalid: err => BadRequest(err),
               Valid: result => result.Match
               (
                  Exception: _ => InternalServerError(Errors.UnexpectedError),
                  Success: _ => Ok()
               )
            );
   }

   public abstract record ActionResult(int StatusCode);
   sealed record OkResult() : ActionResult(StatusCodes.Status200OK);
   sealed record OkObjectResult(object Value) : ActionResult(StatusCodes.Status200OK);
   sealed record BadRequestObjectResult(object Errors) : ActionResult(StatusCodes.Status400BadRequest);
   sealed record InternalServerError(object Error) : ActionResult(StatusCodes.Status500InternalServerError);

   static class ActionResultFactory
   {
      public static ActionResult Ok() => new OkResult();
      public static ActionResult Ok(object value) => new OkObjectResult(value);
      public static ActionResult BadRequest(object error) => new BadRequestObjectResult(error);
      public static ActionResult InternalServerError(object value) => new InternalServerError(value);
   }
}
