using LaYumba.Functional;
using System.Collections.Specialized;
using static LaYumba.Functional.F;

namespace Examples.Chapter5
{
   public static class NameValueCollectionExt
   {
      public static Option<string> Lookup
         (this NameValueCollection @this, string key)
         => @this[key];
   }
}
