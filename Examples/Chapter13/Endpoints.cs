using Boc.Commands;
using Boc.Domain;
using Boc.Domain.Events;

using System;
using Microsoft.AspNetCore.Builder;

using LaYumba.Functional;

using AccountState = Boc.Chapter13.Domain.AccountState;
using Boc.Chapter13.Domain;

using Microsoft.AspNetCore.Http;
using static Microsoft.AspNetCore.Http.Results;

namespace Boc.Chapter13
{
   namespace Api.Unsafe
   {
      public static class Program
      {
         public static MinimalActionEndpointConventionBuilder ConfigureMakeTransferEndpoint
         (
            WebApplication app,
            Func<Guid, AccountState> getAccount,
            Action<Event> saveAndPublish
         )
         => app.MapPost("/Transfer/Make", (Func<MakeTransfer, IResult>)((MakeTransfer cmd) =>
         {
            var account = getAccount(cmd.DebitedAccountId);

            // performs the transfer
            var (evt, newState) = account.Debit(cmd);

            saveAndPublish(evt);

            // returns information to the user about the new state
            return Ok(new { newState.Balance });
         }));
      }


      // unsafe version
      public static class Account
      {
         // handle commands

         public static (Event Event, AccountState NewState) Debit
            (this AccountState @this, MakeTransfer transfer)
         {
            var evt = transfer.ToEvent();
            var newState = @this.Apply(evt);

            return (evt, newState);
         }

         // apply events

         public static AccountState Create(CreatedAccount evt)
            => new AccountState
               (
                  Currency: evt.Currency,
                  Status: AccountStatus.Active
               );

         public static AccountState Apply(this AccountState acc, Event evt)
            => evt switch
            {
               (DepositedCash e) => acc with { Balance = acc.Balance + e.Amount },
               (DebitedTransfer e) => acc with { Balance = acc.Balance - e.DebitedAmount },
               (FrozeAccount e) => acc with { Status = AccountStatus.Frozen },
               _ => throw new InvalidOperationException()
            };
      }
   }

   namespace WithValidation
   {

      public static class Program
      {
         public static MinimalActionEndpointConventionBuilder ConfigureMakeTransferEndpoint
         (
            WebApplication app,
            Func<MakeTransfer, Validation<MakeTransfer>> validate,
            Func<Guid, Option<AccountState>> getAccount,
            Action<Event> saveAndPublish
         )
         => app.MapPost("/Transfer/Make", (Func<MakeTransfer, IResult>)((MakeTransfer transfer)
            => validate(transfer)
               .Bind(t => getAccount(t.DebitedAccountId)
                  .ToValidation($"No account found for {t.DebitedAccountId}"))
               .Bind(acc => acc.Debit(transfer))
               .Do(result => saveAndPublish(result.Event))
               .Match(
                  Invalid: errs => BadRequest(new { Errors = errs }),
                  Valid: result => Ok(new { result.NewState.Balance }))));
      }
   }
}
