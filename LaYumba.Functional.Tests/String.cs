using Xunit;

namespace LaYumba.Functional.Tests
{
   public class String_Test
   {
      [Fact]
      public void PartitionStart()
      {
         var (l, r) = "eurusd".SplitAt(0);
         Assert.Equal(string.Empty, l);
         Assert.Equal("eurusd", r);
      }

      [Fact]
      public void PartitionMiddle()
      {
         var (l, r) = "eurusd".SplitAt(3);
         Assert.Equal("eur", l);
         Assert.Equal("usd", r);
      }

      [Fact]
      public void PartitionEnd()
      {
         var (l, r) = "eurusd".SplitAt(6);
         Assert.Equal("eurusd", l);
         Assert.Equal(string.Empty, r);
      }
   }
}