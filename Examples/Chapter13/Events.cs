using System;

namespace Boc.Domain.Events
{
   public abstract record Event
   (
      Guid EntityId,
      DateTime Timestamp
   );

   public record CreatedAccount
   (
      Guid EntityId,
      DateTime Timestamp,
      CurrencyCode Currency
   )
   : Event(EntityId, Timestamp);

   public record AlteredOverdraft
   (
      Guid EntityId,
      DateTime Timestamp,
      decimal By
   )
   : Event(EntityId, Timestamp);

   public record FrozeAccount
   (
      Guid EntityId,
      DateTime Timestamp
   )
   : Event(EntityId, Timestamp);
   
   public record DepositedCash
   (
      Guid EntityId,
      DateTime Timestamp,
      decimal Amount,
      Guid BranchId
   )
   : Event(EntityId, Timestamp);

   public record DebitedTransfer
   (
      Guid EntityId,
      DateTime Timestamp,
      string Beneficiary,
      string Iban,
      string Bic,
      decimal DebitedAmount,
      string Reference
   )
   : Event(EntityId, Timestamp);

   public record DebitedFee
   (
      Guid EntityId,
      DateTime Timestamp,
      decimal Amount,
      string Description
   )
   : Event(EntityId, Timestamp);
}
