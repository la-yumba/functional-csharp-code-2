namespace Examples.Chapter3
{
   public static class StringExt
   {
      public static string ToSentenceCase(this string s)
         => s == string.Empty
            ? string.Empty
            : char.ToUpperInvariant(s[0]) + s.ToLower()[1..];
   }
}