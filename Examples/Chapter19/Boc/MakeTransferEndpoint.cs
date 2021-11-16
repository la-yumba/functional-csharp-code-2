using System;
using System.Threading.Tasks;

using Boc.Domain;
using Boc.Commands;
using Boc.Chapter13.Domain;
using Boc.Domain.Events;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;

using LaYumba.Functional;
using static LaYumba.Functional.F;

using Microsoft.AspNetCore.Http;
using static Microsoft.AspNetCore.Http.Results;

namespace Boc.Chapter19
{
   public static class Program
   {
      public static void ConfigureMakeTransferEndpoint
      (
         this WebApplication app,
         Validator<MakeTransfer> validate,
         AccountRegistry accounts
      )
      {
         // Func<Guid, Task<Validation<AccountProcess>>>
         var getAccountVal = (Guid id) => accounts.Lookup(id)
               .Map(opt => opt.ToValidation(Errors.UnknownAccountId(id)));

         app.MapPost("/Transfer/Make", (Func<MakeTransfer, Task<IResult>>)((MakeTransfer transfer) =>
         {
            Task<Validation<AccountState>> outcome =
               from cmd in Async(validate(transfer))
               from acc in getAccountVal(cmd.DebitedAccountId)
               from result in acc.Handle(cmd)
               select result.NewState;

            return outcome.Map(
              Faulted: ex => StatusCode(StatusCodes.Status500InternalServerError),
              Completed: val => val.Match(
                 Invalid: errs => BadRequest(new { Errors = errs }),
                 Valid: newState => Ok(new { Balance = newState.Balance })));
         }));
      }
   }

   // same code, but using asp.net controller
   public class MakeTransferController : ControllerBase
   {
      public MakeTransferController(Func<Guid, Task<Option<AccountProcess>>> getAccount
         , Func<MakeTransfer, Validation<MakeTransfer>> validate)
      {
         Validate = validate;
         GetAccount = id => getAccount(id)
            .Map(opt => opt.ToValidation(() => Errors.UnknownAccountId(id)));
      }

      Func<MakeTransfer, Validation<MakeTransfer>> Validate;
      Func<Guid, Task<Validation<AccountProcess>>> GetAccount;
      
      public Task<IActionResult> MakeTransfer([FromBody] MakeTransfer command)
      {
         Task<Validation<AccountState>> outcome =
            from cmd in Async(Validate(command))
            from acc in GetAccount(cmd.DebitedAccountId)
            from result in acc.Handle(cmd)
            select result.NewState;

         return outcome.Map(
            Faulted: ex => StatusCode(500, Errors.UnexpectedError),
            Completed: val => val.Match(
               Invalid: errs => BadRequest(new { Errors = errs }),
               Valid: newState => Ok(new { Balance = newState.Balance }) as IActionResult));
      }
   }   
}
