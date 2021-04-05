using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Examples.Chapter3.ListFormatter.Parallel.Naive
{
   class ListFormatter
   {
      int counter;

      Numbered<T> ToNumberedValue<T>(T t) => new (t, ++counter);

      // possible fix, using lock
      // Numbered<T> ToNumberedValue<T>(T t) => new Numbered<T>(t, Interlocked.Increment(ref counter));

      string Render(Numbered<string> s) => $"{s.Number}. {s.Value}";

      string PrependCounter(string s) => $"{++counter}. {s}";

      public List<string> Format(List<string> list)
         => list
            .AsParallel()
            .Select(StringExt.ToSentenceCase)
            .Select(PrependCounter)
            .ToList();

      public static void Run()
      {
         var size = 1000000;
         var shoppingList = Enumerable.Range(1, size).Select(i => $"item{i}").ToList();

         new ListFormatter()
            .Format(shoppingList)
            .ForEach(Console.WriteLine);

         Console.Read();
      }
   }

   
   public class ParallelListFormatterTests
   {
      [Test]
      public void ItWorksOnSingletonList()
      {
         var input = new List<string>() { "coffee beans" };
         var output = new ListFormatter().Format(input);
         Assert.That("1. Coffee beans" == output[0]);
      }

      //Expected string length 20 but was 19. Strings differ at index 0.
      //Expected: "1000000. Item1000000"
      //But was:  "956883. Item1000000"
      //-----------^
      [Ignore("Tests will fail because of lost updates")]
      [Test]
      public void ItWorksOnAVeryLongList()
      {
         var size = 100000;
         var input = Enumerable.Range(1, size).Select(i => $"item {i}").ToList();
         var output = new ListFormatter().Format(input);
         Assert.AreEqual("100000. Item 100000", output[size - 1]);
      }
   }
}