using Boc.Commands;
using Boc.Domain;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Unit = System.ValueTuple;
using System.Text.RegularExpressions;
using System;
using Examples.Chapter2.DbLogger;

namespace Boc.ValidImpl
{
   public class Chapter6_MakeTransferController_WithValidation : ControllerBase
   {
      ILogger<Chapter6_MakeTransferController_WithValidation> logger;

      [HttpPost, Route("api/Chapters6/transfers/future/particularized")]
      public IActionResult MakeFutureTransfer([FromBody] MakeTransfer request)
         => Handle(request).Match(
            Invalid: BadRequest,
            Valid: result => result.Match(
               Exception: OnFaulted,
               Success: _ => Ok()));

      IActionResult OnFaulted(Exception ex)
      {
         logger.LogError(ex.Message);
         return StatusCode(500, Errors.UnexpectedError);
      }

      Validation<Exceptional<Unit>> Handle(MakeTransfer request)
         => Validate(request)
            .Map(Save);

      Validation<MakeTransfer> Validate(MakeTransfer cmd)
         => ValidateBic(cmd).Bind(ValidateDate);


      // bic code validation

      static readonly Regex regex = new Regex("^[A-Z]{6}[A-Z1-9]{5}$");

      Validation<MakeTransfer> ValidateBic(MakeTransfer transfer)
         => regex.IsMatch(transfer.Bic.ToUpper())
            ? transfer
            : Errors.InvalidBic;

      // date validation

      DateTime now;

      Validation<MakeTransfer> ValidateDate(MakeTransfer transfer)
         => transfer.Date.Date > now.Date
            ? transfer
            : Errors.TransferDateIsPast;

        // persistence

        string connString;

      Exceptional<Unit> Save(MakeTransfer transfer)
      {
         try
         {
            ConnectionHelper.Connect(connString
               , c => c.Execute("INSERT ...", transfer));
         }
         catch (Exception ex) { return ex; }
         return Unit();
      }
   }
}
