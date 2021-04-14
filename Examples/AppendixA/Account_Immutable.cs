using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Boc.Domain;

namespace Examples.AppendixA.Immutable
{
   public sealed class AccountState
   {
      public AccountStatus Status { get; }
      public CurrencyCode Currency { get; }
      public decimal AllowedOverdraft { get; }
      public IEnumerable<Transaction> TransactionHistory { get; }

      public AccountState(CurrencyCode Currency
         , AccountStatus Status = AccountStatus.Requested
         , decimal AllowedOverdraft = 0
         , IEnumerable<Transaction> Transactions = null)
      {
         this.Status = Status;
         this.Currency = Currency;
         this.AllowedOverdraft = AllowedOverdraft;
         this.TransactionHistory = ImmutableList.CreateRange
            (Transactions ?? Enumerable.Empty<Transaction>());
      }

      public AccountState Add(Transaction t)
         => new AccountState(
               Transactions: TransactionHistory.Prepend(t),
               Currency: this.Currency,
               Status: this.Status,
               AllowedOverdraft: this.AllowedOverdraft
            );

      public AccountState WithStatus(AccountStatus newStatus)
         => new AccountState(
               Status: newStatus,
               Currency: this.Currency,
               AllowedOverdraft: this.AllowedOverdraft,
               Transactions: this.TransactionHistory
            );

      public AccountState With
         ( AccountStatus? Status = null 
         , decimal? AllowedOverdraft = null)
         => new AccountState(
               Status: Status ?? this.Status,
               Currency: this.Currency,
               AllowedOverdraft: AllowedOverdraft ?? this.AllowedOverdraft,
               Transactions: this.TransactionHistory
            );
   }

   public class Transaction
   {
      public decimal Amount { get; private set; }
      public string Description { get; private set; }
      public DateTime Date { get; private set; }
   }

   static class Usage
   {
      public static AccountState Freeze(this AccountState account)
         => account.With(Status: AccountStatus.Frozen);

      public static AccountState PutOnAlert(this AccountState account)
         => account.With
         (
            Status: AccountStatus.Frozen,
            AllowedOverdraft: 0m
         );

      static void Run()
      {
         var account = new AccountState
         (
            Currency: "EUR",
            Status: AccountStatus.Active
         );
         var newState = account.WithStatus(AccountStatus.Frozen);

         var frozen = account.With
         (
            Status: AccountStatus.Frozen,
            AllowedOverdraft: 0m
         );
      }
   }
}