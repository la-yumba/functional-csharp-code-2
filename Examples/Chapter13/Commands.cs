using Boc.Domain;
using Boc.Domain.Events;
using System;

namespace Boc.Commands
{
   public record CreateAccount
   (
      DateTime Timestamp,
      Guid AccountId,
      CurrencyCode Currency
   )
      : Command(Timestamp)
   {
      public CreatedAccount ToEvent() => new
      (
         EntityId: this.AccountId,
         Timestamp: this.Timestamp,
         Currency: this.Currency
      );
   }

   public record AcknowledgeCashDeposit
   (
      DateTime Timestamp,
      Guid AccountId,
      decimal Amount,
      Guid BranchId
   )
      : Command(Timestamp)
   {
      public DepositedCash ToEvent() => new 
      (
         EntityId: this.AccountId,
         Timestamp: this.Timestamp,
         Amount: this.Amount,
         BranchId: this.BranchId
      );
   }

   public record SetOverdraft
   (
      DateTime Timestamp,
      Guid AccountId,
      decimal Amount
   )
      : Command(Timestamp)
   {
      public AlteredOverdraft ToEvent(decimal by) => new
      (
         EntityId: this.AccountId,
         Timestamp: this.Timestamp,
         By: by
      );
   }

   public record FreezeAccount
   (
      DateTime Timestamp,
      Guid AccountId
   )
      : Command(Timestamp)
   {
      public FrozeAccount ToEvent() => new
      (
         EntityId: this.AccountId,
         Timestamp: this.Timestamp
      );
   }
}