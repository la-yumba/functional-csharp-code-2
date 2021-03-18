using System;

namespace LaYumba.Functional
{
   using static F;

   public static class NullableExt
   {
      public static Option<T> ToOption<T>(this Nullable<T> @this) where T : struct
         => @this.HasValue ? Some(@this.Value) : None;
   }
}
