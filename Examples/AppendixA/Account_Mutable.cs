using System.Collections.Generic;
using Boc.Domain;

namespace Examples.AppendixA.ByConvention
{
   public class AccountState
   {
      public AccountStatus Status { get; set; }
      public CurrencyCode Currency { get; set; }
      public decimal AllowedOverdraft { get; set; }
      public List<Transaction> TransactionHistory { get; set; }

      public AccountState()
      {
         TransactionHistory = new List<Transaction>();
      }
      public AccountState WithStatus(AccountStatus newStatus)
         => new AccountState
         {
            Status = newStatus,
            Currency = this.Currency,
            AllowedOverdraft = this.AllowedOverdraft,
            TransactionHistory = this.TransactionHistory
         };
   }

   class Usage
   {
      void Run()
      {
         var account = new AccountState
         {
            Status = AccountStatus.Active
         };
         var newState = account.WithStatus(AccountStatus.Frozen);
      }
   }
}
