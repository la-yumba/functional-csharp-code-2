using Boc.Commands;
using Boc.Services;

using NUnit.Framework;
using System;

namespace Examples.Chapter03.Boc.InjectInterface
{
   // interface
   public interface IDateTimeService
   {
      DateTime UtcNow { get; }
   }

   // "real" implementation
   public class DefaultDateTimeService : IDateTimeService
   {
      public DateTime UtcNow => DateTime.UtcNow;
   }

   // testable class depends on interface
   public class DateNotPastValidator_Testable : IValidator<MakeTransfer>
   {
      private readonly IDateTimeService dateService;

      public DateNotPastValidator_Testable(IDateTimeService dateService)
      {
         this.dateService = dateService;
      }

      public bool IsValid(MakeTransfer request)
         => dateService.UtcNow.Date <= request.Date.Date;
   }

   // testable record removes the need for a trivial constructor
   public record DateNotPastValidator_Record(IDateTimeService DateService)
      : IValidator<MakeTransfer>
   {
      // we want our dependency to be private (by default it would be public)
      private IDateTimeService DateService { get; init; } = DateService;

      public bool IsValid(MakeTransfer request)
         => DateService.UtcNow.Date <= request.Date.Date;
   }

   // can be tested using fakes
   public class DateNotPastValidatorTest
   {
      static DateTime presentDate = new DateTime(2016, 12, 12);

      // "fake" implementation
      private class FakeDateTimeService : IDateTimeService
      {
         public DateTime UtcNow => presentDate;
      }

      // example-based unit test
      [Test]
      public void WhenTransferDateIsPast_ThenValidatorFails()
      {
         var sut = new DateNotPastValidator_Testable(new FakeDateTimeService());
         var transfer = MakeTransfer.Dummy with
         {
            Date = presentDate.AddDays(-1)
         };
         Assert.AreEqual(false, sut.IsValid(transfer));
      }

      // parameterized unit test
      [TestCase(+1, ExpectedResult = true)]
      [TestCase(0, ExpectedResult = true)]
      [TestCase(-1, ExpectedResult = false)]
      public bool WhenTransferDateIsPast_ThenValidationFails(int offset)
      {
         var sut = new DateNotPastValidator_Testable(new FakeDateTimeService());
         var transferDate = presentDate.AddDays(offset);
         var transfer = MakeTransfer.Dummy with { Date = transferDate };

         return sut.IsValid(transfer);
      }
   }
}