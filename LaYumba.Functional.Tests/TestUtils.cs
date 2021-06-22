using Xunit;

namespace LaYumba.Functional.Tests
{
   public static class TestUtils
   {
      public static void Fail() => Assert.True(false);
   }
}
