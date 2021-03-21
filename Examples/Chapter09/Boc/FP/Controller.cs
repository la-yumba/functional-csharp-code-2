using Boc.Commands;
using Boc.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using NUnit.Framework;
using Unit = System.ValueTuple;

namespace Boc.Chapter9
{
   public delegate Validation<T> Validator<T>(T t);

   // option 2. use MVC, and inject a handler function as a dependency
   namespace Delegate
   {
      public static class Validation
      {
         public static Validator<MakeTransfer> DateNotPast(Func<DateTime> clock)
            => cmd 
            => cmd.Date.Date < clock().Date
               ? Errors.TransferDateIsPast
               : Valid(cmd);
      }

      public class MakeTransferController_FunctionDependencies : ControllerBase
      {
         Validator<MakeTransfer> validate;
         Func<MakeTransfer, Exceptional<Unit>> save;

         public MakeTransferController_FunctionDependencies(Validator<MakeTransfer> validate
            , Func<MakeTransfer, Exceptional<Unit>> save)
         {
            this.validate = validate;
            this.save = save;
         }

         //[HttpPut, Route("api/TransferOn")]
         public IActionResult MakeTransfer([FromBody] MakeTransfer cmd)
            => validate(cmd).Map(save).Match( 
               Invalid: BadRequest,
               Valid: result => result.Match<IActionResult>(
                  Exception: _ => StatusCode(500, Errors.UnexpectedError),
                  Success: _ => Ok()));
      }

      public class Tests
      {
         [Test]
         public void WhenValid_AndSaveSucceeds_ThenResponseIsOk()
         {
            var controller = new MakeTransferController_FunctionDependencies(
               validate: transfer => Valid(transfer),
               save: _ => Exceptional(Unit()));

            var result = controller.MakeTransfer(MakeTransfer.Dummy);

            Assert.AreEqual(typeof(OkResult), result.GetType());
         }
      }
   }

   // option 3. dont use MVC; just use a function, and inject it into 
   // the pipeline in the Startup class
   namespace FunctionsEverywhere
   {
      using static ActionResultFactory;

      public class UseCases
      {
         public static Func<ILogger
            , Func<MakeTransfer, Validation<Exceptional<Unit>>>
            , MakeTransfer
            , IActionResult>
         TransferOn => (logger, handle, cmd) =>
         {
            Func<Exception, IActionResult> onFaulted = ex =>
            {
               logger.LogError(ex.Message);
               return InternalServerError(ex);
            };

            return handle(cmd).Match(
               Invalid: BadRequest,
               Valid: result => result.Match(
                  Exception: onFaulted,
                  Success: _ => Ok()));
         };
      }

      static class ActionResultFactory
      {
         public static IActionResult Ok() => new OkResult();
         public static IActionResult Ok(object value) => new OkObjectResult(value);
         public static IActionResult BadRequest(object error) => new BadRequestObjectResult(error);
         public static IActionResult InternalServerError(object value)
         {
            var result = new ObjectResult(value);
            result.StatusCode = 500;
            return result;
         }
      }
   }
}
