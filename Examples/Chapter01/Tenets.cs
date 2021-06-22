using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Examples.Chapter1
{
   using static Enumerable;
   using static Console;

   class FunctionsAsFirstClassValues
   {
      [Test]
      public static void Run()
      {
         static int triple(int x) => x * 3;
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
         var original = new[] { 5, 7, 1 };
         var sorted = original.OrderBy(x => x).ToList();

         Assert.AreEqual(new[] { 5, 7, 1 }, original);
         Assert.AreEqual(new[] { 1, 5, 7 }, sorted);
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
         Action task1 = () => WriteLine(nums.Sum());
         Action task3 = () => WriteLine(nums.OrderBy(x => x).Sum());

         Parallel.Invoke(task1, task3);
      }
   }
}