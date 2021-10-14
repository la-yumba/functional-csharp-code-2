using static System.Math;

namespace Examples.Chapter13.Data
{
   namespace WithRecords
   {
      readonly record struct Point(double X, double Y);
      readonly record struct Circle(Point Center, double Radius);

      static class CircleExt
      {
         static Circle Scale(this Circle c, double factor)
            => c with { Radius = c.Radius * factor };
      }
   }

   namespace BeforeRecords
   {
      struct Circle
      {
         public Circle(Point center, double radius)
            => (Center, Radius) = (center, radius);

         public Point Center { get; init; }
         public double Radius { get; init; }

         public double Area => PI * Pow(Radius, 2);
      }

      struct Point
      {
         public double X { get; }
         public double Y { get; }
         public Point(double x, double y) { X = x; Y = y; }
      }

      static class CircleExt
      {
         static Circle Scale(this Circle @this, double factor)
            => new Circle(@this.Center, @this.Radius * factor);

         static Circle Scale_CSharp10(this Circle c, double factor)
            => c with { Radius = c.Radius * factor };
      }
   }
}
