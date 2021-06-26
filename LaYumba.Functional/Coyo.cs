using System;

namespace LaYumba.Functional
{
   public record Coyo<V, T>(V Value, Func<object, T> Func);

   public static class Coyo
   {
      public static Coyo<V, T> Of<V, T>(V value)
         => new(value, x => (T)x);

      public static Coyo<V, R> Map<V, T, R>(this Coyo<V, T> @this
         , Func<T, R> func)
         => new(@this.Value, x => func(@this.Func(x)));
   }
}
