using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

using LaYumba.Functional;

namespace Examples.FunctionalApi
{
   public static class Extensions
   {
      public static WebApplication MapPost<T>
      (
         this WebApplication app,
         string route,
         Func<T, ActionResult> handle
      )
      => app.MapPost(route, handle.Map(Task.FromResult));

      //{
      //   app.MapPost(route, async http =>
      //   {
      //      T t = await http.Request.ReadFromJsonAsync<T>();

      //      ActionResult result = handle(t);

      //      http.Response.StatusCode = result.StatusCode;

      //      await http.Response.WriteAsJsonAsync(result switch
      //      {
      //         OkObjectResult r => r.Value,
      //         BadRequestObjectResult r => r.Errors,
      //         _ => string.Empty
      //      });
      //   });

      //   return app;
      //}

      public static WebApplication MapPost<T>
      (
         this WebApplication app,
         string route,
         Func<T, Task<ActionResult>> handle
      )
      {
         app.MapPost(route, async http =>
         {
            T t = await http.Request.ReadFromJsonAsync<T>();

            ActionResult result = await handle(t);

            http.Response.StatusCode = result.StatusCode;

            await http.Response.WriteAsJsonAsync(result switch
            {
               OkObjectResult r => r.Value,
               BadRequestObjectResult r => r.Errors,
               _ => string.Empty
            });
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
