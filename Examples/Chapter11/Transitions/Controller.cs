using Boc.Commands;
using Boc.Domain;
using System;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Boc.Domain.Events;
using AccountState = Boc.Chapter11.Domain.AccountState;
using Microsoft.AspNetCore.Mvc;

namespace Boc.Chapter11.Transitions
{
   namespace WithValidation
   {
      public class Chapter10_Transfers_WithValidation : ControllerBase
      {
         Func<CreateAccountWithOptions, Validation<CreateAccountWithOptions>> validate;
         Func<Guid> generateId;
         Action<Event> saveAndPublish;
         
         public IActionResult CreateInitialized([FromBody] CreateAccountWithOptions cmd)
            => validate(cmd)
               .Bind(Initialize)
               .Match<IActionResult>(
                  Invalid: errs => BadRequest(new { Errors = errs }),
                  Valid: id => Ok(id));

         Validation<Guid> Initialize(CreateAccountWithOptions cmd)
         {
            Guid id = generateId();
            DateTime now = DateTime.UtcNow;

            var create = new CreateAccount
            (
               Timestamp: now,
               AccountId: id,
               Currency: cmd.Currency
            );

            var depositCash = new AcknowledgeCashDeposit
            (
               Timestamp: now,
               AccountId: id,
               Amount: cmd.InitialDepositAccount,
               BranchId: cmd.BranchId
            );

            var setOverdraft = new SetOverdraft
            (
               Timestamp: now,
               AccountId: id,
               Amount: cmd.AllowedOverdraft
            );

            var transitions =
               from e1 in Account.Create(create)
               from e2 in Account.Deposit(depositCash)
               from e3 in Account.SetOverdraft(setOverdraft)
               select List<Event>(e1, e2, e3);

            return transitions(default(AccountState))
               .Do(t => t.Item1.ForEach(saveAndPublish))
               .Map(_ => id);
         }
      }

      public record CreateAccountWithOptions
      (
         DateTime Timestamp,

         CurrencyCode Currency,
         decimal InitialDepositAccount,
         decimal AllowedOverdraft,
         Guid BranchId
      )
         : Command(Timestamp);
   }
}
