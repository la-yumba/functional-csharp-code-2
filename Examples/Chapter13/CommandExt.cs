using Boc.Commands;
using Boc.Domain.Events;

namespace Boc.Chapter13
{
   public static class CommandExt
   {
      public static DebitedTransfer ToEvent(this MakeTransfer cmd) => new
      (
         Beneficiary: cmd.Beneficiary,
         Bic: cmd.Bic,
         DebitedAmount: cmd.Amount,
         EntityId: cmd.DebitedAccountId,
         Iban: cmd.Iban,
         Reference: cmd.Reference,
         Timestamp: cmd.Timestamp
      );
   }
}
