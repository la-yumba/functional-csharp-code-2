using Microsoft.AspNetCore.Mvc;

// TODO this will eventually be added in Microsoft.AspNetCore.Http and should therefore be removed
// see https://github.com/dotnet/aspnetcore/issues/33729

namespace Microsoft.AspNetCore.Http
{
   public static class Results
   {
      public static IResult NotFound() => new StatusCodeResult(404);
      public static IResult Ok() => new StatusCodeResult(200);
      public static IResult Status(int statusCode) => new StatusCodeResult(statusCode);
      public static IResult Ok(object value) => new JsonResult(value);

      // TODO pass the value along
      public static IResult BadRequest(object value) => new BadRequestResult(); 
      public static IResult InternalServerError(object value) => new StatusCodeResult(StatusCodes.Status500InternalServerError);
   }
}
