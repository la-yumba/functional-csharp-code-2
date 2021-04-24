using Boc.Domain;

namespace Boc.Domain
{
   public enum AccountStatus
   { Requested, Active, Frozen, Dormant, Closed }
}

namespace Boc.Chapter13.Domain
{
   public sealed record AccountState
   (
      CurrencyCode Currency,
      AccountStatus Status = AccountStatus.Requested,
      decimal Balance = 0,
      decimal AllowedOverdraft = 0
   );
}
