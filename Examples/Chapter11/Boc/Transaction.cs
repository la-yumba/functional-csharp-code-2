using System;

namespace Boc.Domain
{
   public record Transaction
   (
      decimal Amount,
      string Description,
      DateTime Date
   );
}