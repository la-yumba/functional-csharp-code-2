using Boc.Domain.Events;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Boc.Chapter13.Query
{
   public record Transaction
   (
       DateTime Date,
       string Description,
       decimal DebitedAmount = 0m,
       decimal CreditedAmount = 0m
   );

   public record AccountStatement
   (
      int Month,
      int Year,
      decimal StartingBalance,
      decimal EndBalance,
      IEnumerable<Transaction> Transactions
   )
   {
      public static AccountStatement Create(int month, int year, IEnumerable<Event> events)
      {
         var startOfPeriod = new DateTime(year, month, 1);
         var endOfPeriod = startOfPeriod.AddMonths(1);

         var (eventsBeforePeriod, eventsDuringPeriod) = events
            .TakeWhile(e => endOfPeriod < e.Timestamp)
            .Partition(e => e.Timestamp <= startOfPeriod);

         var startingBalance = eventsBeforePeriod.Aggregate(0m, BalanceReducer);
         var endBalance = eventsDuringPeriod.Aggregate(startingBalance, BalanceReducer);

         return new
         (
            Month: month,
            Year: year,
            StartingBalance: startingBalance,
            EndBalance: endBalance,
            Transactions: eventsDuringPeriod.Bind(CreateTransaction)
         );
      }

      static Option<Transaction> CreateTransaction(Event evt)
         => evt switch
         {
            DepositedCash e
               => new Transaction
                  (
                     CreditedAmount: e.Amount,
                     Description: $"Deposit at {e.BranchId}",
                     Date: e.Timestamp.Date
                  ),

            DebitedTransfer e
               => new Transaction
                  (
                     DebitedAmount: e.DebitedAmount,
                     Description: $"Transfer to {e.Bic}/{e.Iban}; Ref: {e.Reference}",
                     Date: e.Timestamp.Date
                  ),

            _ => None
         };

      static decimal BalanceReducer(decimal bal, Event evt)
         => evt switch
         {
            DepositedCash e => bal + e.Amount,
            DebitedTransfer e => bal - e.DebitedAmount,
            _ => bal
         };
   }
}
