using System;
using LaYumba.Functional;

namespace Boc.Domain
{
   public static class Errors
   {
      public static Error InsufficientBalance
         => new InsufficientBalanceError();

      public static Error InvalidBic
         => new InvalidBicError();

      public static Error CannotActivateClosedAccount
         => new CannotActivateClosedAccountError();

      public static Error TransferDateIsPast 
         => new TransferDateIsPastError();

      public static Error AccountNotActive
         => new AccountNotActiveError();

      public static Error UnexpectedError
         => new UnexpectedError();

      public static Error UnknownAccountId(Guid id)
         => new UnknownAccountIdError(id);
   }

   public sealed record UnknownAccountIdError(Guid Id)
      : Error($"No account with id {Id} was found");

   public sealed record UnexpectedError()
      : Error("An unexpected error has occurred");

   public sealed record AccountNotActiveError()
      : Error("The account is not active; the requested operation cannot be completed");

   public sealed record InvalidBicError()
      : Error("The beneficiary's BIC/SWIFT code is invalid");

   public sealed record InsufficientBalanceError()
      : Error("Insufficient funds to fulfil the requested operation");

   public sealed record CannotActivateClosedAccountError()
      : Error("Cannot activate an account that has been closed");

   public sealed record TransferDateIsPastError()
      : Error("Transfer date cannot be in the past");
}
