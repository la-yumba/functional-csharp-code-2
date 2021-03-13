using System;
using Boc.Commands;
using Boc.Services;

namespace Examples.Chapter03.Boc.InjectValue
{
   public record DateNotPastValidator(DateTime Today)
      : IValidator<MakeTransfer>
   {
      public bool IsValid(MakeTransfer cmd)
         => Today <= cmd.Date.Date;
   }
}