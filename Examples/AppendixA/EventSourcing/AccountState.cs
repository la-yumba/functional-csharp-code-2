using Boc.Domain;

namespace Boc.AppendixA.EventSourcing.Domain
{
   public sealed class AccountState
   {
      public AccountStatus Status { get; }
      public CurrencyCode Currency { get; }
      public decimal Balance { get; }
      public decimal AllowedOverdraft { get; }

      public AccountState
      (
         CurrencyCode Currency,
         AccountStatus Status = AccountStatus.Requested,
         decimal Balance = 0m,
         decimal AllowedOverdraft = 0m
      )
      {
         this.Currency = Currency;
         this.Status = Status;
         this.Balance = Balance;
         this.AllowedOverdraft = AllowedOverdraft;
      }

      public AccountState WithStatus(AccountStatus newStatus)
         => new AccountState
            (
               Status: newStatus,
               Balance: this.Balance,
               Currency: this.Currency,
               AllowedOverdraft: this.AllowedOverdraft
            );

      public AccountState Debit(decimal amount)
         => Credit(-amount);

      public AccountState Credit(decimal amount)
         => new AccountState
            (
               Balance: this.Balance + amount,
               Currency: this.Currency,
               Status: this.Status,
               AllowedOverdraft: this.AllowedOverdraft
            );
   }
}
