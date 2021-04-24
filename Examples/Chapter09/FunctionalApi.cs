using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Examples.FunctionalApi
{
   public static class Extensions
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
