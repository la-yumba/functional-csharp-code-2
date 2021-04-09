using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Boc.Domain;
using NUnit.Framework;

namespace Examples.Chapter11
{
   public record AccountState_Positional
   (
      CurrencyCode Currency,
      AccountStatus Status = AccountStatus.Requested,
      decimal AllowedOverdraft = 0m,
      IEnumerable<Transaction> TransactionHistory = null
   );

   public record AccountState
   {
      public CurrencyCode Currency { get; }
      public AccountStatus Status { get; init; } = AccountStatus.Requested;
      public decimal AllowedOverdraft { get; init; } = 0m;

      private readonly IEnumerable<Transaction> transactions = Enumerable.Empty<Transaction>();
      public IEnumerable<Transaction> TransactionHistory
      {
         get => transactions;
         init { transactions = value?.ToImmutableList() ?? Enumerable.Empty<Transaction>(); }
      }

      //public IEnumerable<Transaction> TransactionHistory { get; init; }
      // = Enumerable.Empty<Transaction>();

      public AccountState(CurrencyCode Currency)
         => this.Currency = Currency;
   }

   public record AccountState_WithCtor
   {
      public CurrencyCode Currency { get; }
      public AccountStatus Status { get; init; } 
      public decimal AllowedOverdraft { get; init; }
      public IEnumerable<Transaction> TransactionHistory { get; init; }

      public AccountState_WithCtor
      (
         CurrencyCode Currency,
         AccountStatus Status = AccountStatus.Requested,
         decimal AllowedOverdraft = 0m,
         IEnumerable<Transaction> TransactionHistory = null
      )
      {
         (this.Currency, this.Status, this.AllowedOverdraft)
            = (Currency, Status, AllowedOverdraft);

         this.TransactionHistory = TransactionHistory?.ToImmutableList()
            ?? Enumerable.Empty<Transaction>();
      }
   }

   public static class Account
   {
      public static AccountState Add(this AccountState original, Transaction transaction)
         => original with
         {
            TransactionHistory = original.TransactionHistory.Prepend(transaction)
         };

      public static AccountState Activate(this AccountState original)
         => original with { Status = AccountStatus.Active };
   }


   public static class Usage
   {
      [Test]
      public static void WithOnlyChangesTheSpecifiedFields()
      {
         var original = new AccountState(Currency: "EUR");
         var activated = original.Activate();

         Assert.AreEqual(AccountStatus.Requested, original.Status);
         Assert.AreEqual(AccountStatus.Active, activated.Status);
         Assert.AreEqual(original.Currency, activated.Currency);
      }

      [Test]
      public static void GivenRecordImmutable_WhenMutateList_ThenRecordIsNotMutated()
      {
         var mutable = new List<Transaction>();
         var account = new AccountState("EUR")
         {
            TransactionHistory = mutable
         };
         mutable.Add(new(-1000, "Create trouble", DateTime.Now));

         Assert.AreEqual(1, mutable.Count());
         Assert.AreEqual(0, account.TransactionHistory.Count());
      }
   }
}