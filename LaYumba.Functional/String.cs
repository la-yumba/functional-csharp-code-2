using System;

namespace LaYumba.Functional
{
   public static class String
   {
      public static Func<string, string> Trim = s => s.Trim();
      public static Func<string, string> ToLower = s => s.ToLower();
      public static Func<string, string> ToUpper = s => s.ToUpper();

      public static (string Left, string Right) SplitAt(this string s, int at)
         => (s.Substring(0, at), s.Substring(at));
   }
}
