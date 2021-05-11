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

using Examples.FunctionalApi;
using static Examples.FunctionalApi.ActionResultFactory;

namespace Boc.Chapter19
{
   public static class Program
   {
      public static WebApplication ConfigureMakeTransferEndpoint
      (
         WebApplication app,
         Validator<MakeTransfer> validate,
         AccountRegistry accounts
      )
      {
         Func<Guid, Task<Validation<AccountProcess>>> getAccountVal
            = id => accounts.Lookup(id)
               .Map(opt => opt.ToValidation(Errors.UnknownAccountId(id)));

         return app.MapPost("/Transfer/Make", (MakeTransfer transfer) =>
         {
            Task<Validation<AccountState>> outcome =
               from cmd in Async(validate(transfer))
               from acc in getAccountVal(cmd.DebitedAccountId)
               from result in acc.Handle(cmd)
               select result.NewState;

            return outcome.Map(
              Faulted: ex => InternalServerError(Errors.UnexpectedError),
              Completed: val => val.Match(
                 Invalid: errs => BadRequest(new { Errors = errs }),
                 Valid: newState => Ok(new { Balance = newState.Balance })));
         });
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
