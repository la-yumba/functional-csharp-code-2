using System;
using LaYumba.Functional;
using NUnit.Framework;
using static System.Math;

namespace Examples.Chapter1
{
   namespace CSharp7
   {
      record Circle(double Radius)
      {
         public (double Circumference, double Area) Stats
            => (Circumference, Area);

         public double Circumference
            => PI * 2 * Radius;

         public double Area
         {
            get
            {
               static double Square(double d) => Pow(d, 2);
               return PI * Square(Radius);
            }
         }
      }
   }

   namespace Tuples
   {
      public static class Example
      {
         public static (string Base, string Quote)
            AsPair(this string ccyPair)
            => ccyPair.SplitAt(3);

         [Test]
         public static void Test()
         {
            var pair = "EURUSD".AsPair();
            Assert.AreEqual("EUR", pair.Base);
            Assert.AreEqual("USD", pair.Quote);
         }
      }
   }

   namespace CSharp9
   {
      interface Shape { }

      record Rectangle(double Length, double Height) : Shape;

      record Circle(double Radius) : Shape;

      static class Geometry
      {
         public static double Area(this Shape shape)
            => shape switch
            {
               Circle(var r) => PI * Pow(r, 2),
               Rectangle(var l, var h) => l * h,
               _ => throw new ArgumentException("unknown shape"),
            };
      }
   }
}
