using System;
using NUnit.Framework;

namespace Examples.Chapter11
{
   class DateTime_Example
   {
      [Test]
      public static void Run()
      {
         var momsBirthday = new DateOnly(1966, 12, 13);
         var johnsBirthday = momsBirthday;

         // some time goes by... 
         johnsBirthday = johnsBirthday.AddDays(1);

         Assert.AreEqual(new DateOnly(1966, 12, 14), johnsBirthday);

         // mom's birthday hasn't been affected
         Assert.AreEqual(new DateOnly(1966, 12, 13), momsBirthday);
      }
   }
}
