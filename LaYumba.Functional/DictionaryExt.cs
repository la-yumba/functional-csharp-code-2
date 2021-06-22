using System.Collections.Generic;

namespace LaYumba.Functional
{
   using static F;

   public static class DictionaryExt
   {
      public static Option<T> Lookup<K, T>(this IDictionary<K, T> dict, K key)
         => dict.TryGetValue(key, out T? value) ? Some(value) : None;
   }
}