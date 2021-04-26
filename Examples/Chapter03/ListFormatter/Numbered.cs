namespace Examples
{
   public record Numbered<T>(T Value, int Number)
   {
      public override string ToString()
         => $"({Number}, {Value})";

      public static Numbered<T> Create(T Value, int Number) => new(Value, Number);
   }
}