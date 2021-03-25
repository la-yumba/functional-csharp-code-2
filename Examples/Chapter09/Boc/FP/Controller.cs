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
      public IActionResult MakeTransfer([FromBody] MakeTransfer transfer)
         => validate(transfer).Map(save).Match(
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

