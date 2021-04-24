using Boc.Domain.Events;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Boc.Chapter13.Query
{
   public record Transaction
   {
      public DateTime Date { get; }
      public decimal DebitedAmount { get; }
      public decimal CreditedAmount { get; }
      public string Description { get; }

      public Transaction(DebitedTransfer e)
      {
         DebitedAmount = e.DebitedAmount;
         Description = $"Transfer to {e.Bic}/{e.Iban}; Ref: {e.Reference}";
         Date = e.Timestamp.Date;
      }

      public Transaction(DepositedCash e)
      {
         CreditedAmount = e.Amount;
         Description = $"Deposit at {e.BranchId}";
         Date = e.Timestamp.Date;
      }
   }

   public record AccountStatement
   {
      public int Month { get; }
      public int Year { get; }

      public decimal StartingBalance { get; }
      public decimal EndBalance { get; }
      public IEnumerable<Transaction> Transactions { get; }

      public AccountStatement(int month, int year, IEnumerable<Event> events)
      {
         Month = month;
         Year = year;

         var startOfPeriod = new DateTime(year, month, 1);
         var endOfPeriod = startOfPeriod.AddMonths(1);

         var eventsBeforePeriod = events
            .TakeWhile(e => e.Timestamp < startOfPeriod);
         var eventsDuringPeriod = events
            .SkipWhile(e => e.Timestamp < startOfPeriod)
            .TakeWhile(e => endOfPeriod < e.Timestamp);

         StartingBalance = eventsBeforePeriod.Aggregate(0m, BalanceReducer);
         EndBalance = eventsDuringPeriod.Aggregate(StartingBalance, BalanceReducer);

         Transactions = eventsDuringPeriod.Bind(CreateTransaction);
      }

      Option<Transaction> CreateTransaction(Event evt)
         => evt switch
         {
            DepositedCash e => new Transaction(e),
            DebitedTransfer e => new Transaction(e),
            _ => None
         };

      decimal BalanceReducer(decimal bal, Event evt)
         => evt switch
         {
            DepositedCash e => bal + e.Amount,
            DebitedTransfer e => bal - e.DebitedAmount,
            _ => bal
         };
   }
}
