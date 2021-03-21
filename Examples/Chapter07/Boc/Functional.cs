using Microsoft.AspNetCore.Mvc;
using System;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Boc.Commands;
using Boc.Services;

namespace Boc.Chapter7
{
   // top-level workflow
   [ApiController, Route("Chapter7/transfer")]
   public class Chapter5_TransfersController : ControllerBase
   {
      IValidator<MakeTransfer> validator;
      IRepository<AccountState> accounts;
      ISwiftService swift;

      [HttpPost]      
      public void MakeTransfer([FromBody] MakeTransfer transfer)
         => Some(transfer)
            .Map(Normalize)
            .Where(validator.IsValid)
            .ForEach(Book);

      void Book(MakeTransfer transfer)
         => accounts.Get(transfer.DebitedAccountId)
            .Bind(account => account.Debit(transfer.Amount))
            .ForEach(newState =>
               {
                  accounts.Save(transfer.DebitedAccountId, newState);
                  swift.Wire(transfer, newState);
               });

      MakeTransfer Normalize(MakeTransfer request) 
         => request; // remove whitespace, toUpper, etc.
   }


   // domain model

   public record AccountState(decimal Balance);

   public static class Account
   {
      public static Option<AccountState> Debit
         (this AccountState current, decimal amount)
         => (current.Balance < amount)
            ? None
            : Some(new AccountState
               (
                  Balance : current.Balance - amount
               )
            );
   }
   

   // dependencies

   public interface IRepository<T>
   {
      Option<T> Get(Guid id);
      void Save(Guid id, T t);
   }

   interface ISwiftService
   {
      void Wire(MakeTransfer transfer, AccountState account);
   }
}