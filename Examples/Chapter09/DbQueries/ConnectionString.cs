namespace Examples
{
   public record ConnectionString(string Value)
   {
      public static implicit operator string(ConnectionString c)
         => c.Value;
      public static implicit operator ConnectionString(string s)
         => new (s);
   }
}
