using System.Collections.Generic;
using System.Threading.Tasks;

using static System.Linq.Enumerable;

namespace Examples.Chapter1;

class FunctionsAsFirstClassValues
{
   [Test]
   public static void Select()
   {
      var triple = (int x) => x * 3;

      // alternatively, before C# 10...
      //Func<int, int> triple = x => x * 3;

      // alternatively, with local function
      //static int triple(int x) => x * 3;

      var range = Range(1, 3);
      var triples = range.Select(triple);

      Assert.AreEqual(new List<int>() { 3, 6, 9 }
         , triples);
   }
}

public class MutationShouldBeAvoided
{      
   [Test]
   public void NoInPlaceUpdates()
   {
      var isOdd = (int x) => x % 2 == 1;
      int[] original = { 7, 6, 1 };

      var sorted = original.OrderBy(x => x);
      var filtered = original.Where(isOdd);

      Assert.AreEqual(new[] { 7, 6, 1 }, original);
      Assert.AreEqual(new[] { 1, 6, 7 }, sorted);
      Assert.AreEqual(new[] { 7, 1 }, filtered);
   }

   [Test]
   public void InPlaceUpdates()
   {
      int[] original = { 5, 7, 1 };
      Array.Sort(original);
      Assert.AreEqual(new[] { 1, 5, 7 }, original);
   }

   static readonly int[] nums = Range(-10000, 20001).Reverse().ToArray();

   public static void WithArrayItBreaks()
   {
      void task1() => WriteLine(nums.Sum());
      void task2() { Array.Sort(nums); WriteLine(nums.Sum()); }

      Parallel.Invoke(task1, task2);
   }

   public static void WithIEnumerableItWorks()
   {
      var task1 = () => WriteLine(nums.Sum());
      var task3 = () => WriteLine(nums.OrderBy(x => x).Sum());

      Parallel.Invoke(task1, task3);
   }
}
