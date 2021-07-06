using Xunit;
using System;
using System.Linq;

namespace LaYumba.Functional.Tests
{
   public class IEnumerable_Partition
   {
      [Fact]
      public void Partition()
      {
         var nums = Enumerable.Range(0, 10);
         var (even, odd) = nums.Partition(i => i % 2 == 0);

         Assert.Equal(new int[] { 0, 2, 4, 6, 8 }, even);
         Assert.Equal(new int[] { 1, 3, 5, 7, 9 }, odd);
      }
   }
}
