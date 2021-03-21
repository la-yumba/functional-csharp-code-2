namespace LaYumba.Functional
{
   public static partial class F
   {
      public static Error Error(string message) => new Error(message);
   }

   public record Error(string Message)
   {
      public override string ToString() => Message;

      public static implicit operator Error(string m) => new(m);
   }
}
