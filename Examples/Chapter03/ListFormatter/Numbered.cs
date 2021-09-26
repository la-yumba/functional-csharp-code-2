namespace Examples
{
   public record Numbered<T>(T Value, int Number)
   {
      public override string ToString()
         => $"({Number}, {Value})";
   }
}