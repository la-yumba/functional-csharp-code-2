using System;
using Boc.Commands;
using Boc.Services;
using NUnit.Framework;

namespace Examples.Chapter03.Boc.InjectFunc
{
   public record DateNotPastValidator(Func<DateTime> Clock)
      : IValidator<MakeTransfer>
   {
      public bool IsValid(MakeTransfer cmd)
         => Clock().Date <= cmd.Date.Date;
   }

   public class DateNotPastValidator_Test
   {
      readonly DateTime today = new(2021, 3, 12);

      [Test]
      public void WhenTransferDateIsToday_ThenValidatorPasses()
      {
         var sut = new DateNotPastValidator(() => today);
         var transfer = MakeTransfer.Dummy with { Date = today };

         Assert.AreEqual(true, sut.IsValid(transfer));
      }
   }
}

namespace Examples.Chapter03.Boc.InjectDelegate
{
   public delegate DateTime Clock();

   public record DateNotPastValidator(Clock Clock)
      : IValidator<MakeTransfer>
   {
      public bool IsValid(MakeTransfer cmd)
         => Clock().Date <= cmd.Date.Date;
   }
}
