namespace Examples.Chapter7
{
   record Circle(Point Center, double Radius);
   record Point(double X, double Y);

   static class Geometry
   {
      static Circle Move(this Circle c, double x, double y) => new 
      (
         Center: new(c.Center.X + x, c.Center.Y + y),
         Radius: c.Radius
      );

      static Circle Scale(this Circle c, double factor) => new 
      (
         Center: c.Center,
         Radius: c.Radius * factor
      );
   }
}
