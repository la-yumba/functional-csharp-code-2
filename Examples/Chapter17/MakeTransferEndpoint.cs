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
using Unit = System.ValueTuple;

using Microsoft.AspNetCore.Http;
using static Microsoft.AspNetCore.Http.Results;

namespace Boc.Chapter17
{
   public static class Program
   {
      public static WebApplication ConfigureMakeTransferEndpoint
      (
         this WebApplication app,
         Validator<MakeTransfer> validate,
         Func<Guid, Task<Option<AccountState>>> getAccount,
         Func<Event, Task> saveAndPublish
      )
      {
         Func<Guid, Task<Validation<AccountState>>> getAccountVal
            = id => getAccount(id)
               .Map(opt => opt.ToValidation(Errors.UnknownAccountId(id)));

         Func<Event, Task<Unit>> saveAndPublishF
            = async e =>
            {
               await saveAndPublish(e);
               return Unit();
            };

         app.MapPost("/Transfer/Make", (Func<MakeTransfer, Task<IResult>>)((MakeTransfer transfer) =>
         {
             Task<Validation<AccountState>> outcome =
                from tr in Async(validate(transfer))
                from acc in getAccountVal(tr.DebitedAccountId)
                from result in Async(Account.Debit(acc, tr))
                from _ in saveAndPublish.ToFunc()(result.Event).Map(Valid)
                select result.NewState;

             return outcome.Map(
               Faulted: ex => InternalServerError(Errors.UnexpectedError),
               Completed: val => val.Match(
                  Invalid: errs => BadRequest(new { Errors = errs }),
                  Valid: newState => Ok(new { Balance = newState.Balance })));
         }));

         return app;
      }
   }

   // same code, but using asp.net ...
   public class TransferNowController : ControllerBase
   {
      Func<MakeTransfer, Validation<MakeTransfer>> validate;
      Func<Guid, Task<Option<AccountState>>> getAccount;
      Func<Event, Task> saveAndPublish;

      Func<Guid, Task<Validation<AccountState>>> GetAccount 
         => id 
         => getAccount(id)
            .Map(opt => opt.ToValidation(() => Errors.UnknownAccountId(id)));

      Func<Event, Task<Unit>> SaveAndPublish => async e =>
      {
         await saveAndPublish(e);
         return Unit();
      };

      public Task<IActionResult> Transfer([FromBody] MakeTransfer command)
      {
         Task<Validation<AccountState>> outcome =
            from cmd in Async(validate(command))
            from acc in GetAccount(cmd.DebitedAccountId)
            from result in Async(Account.Debit(acc, cmd))
            from _ in SaveAndPublish(result.Event).Map(Valid)
            select result.NewState;
         
         //Task<Validation<AccountState>> a = validate(command)
         //   .Traverse(cmd => getAccount(cmd.DebitedAccountId)
         //      .Bind(acc => Account.Debit(acc, cmd)
         //         .Traverse(result => saveAndPublish(result.Event)
         //            .Map(_ => result.NewState))))
         //   .Map(vva => vva.Bind(va => va)); // flatten the nested validation inside the task

         return outcome.Map(
            Faulted: ex => StatusCode(500, Errors.UnexpectedError),
            Completed: val => val.Match(
               Invalid: errs => BadRequest(new { Errors = errs }),
               Valid: newState => Ok(new { Balance = newState.Balance }) as IActionResult));
      }
   }   
}
