namespace Examples
{
   public record SqlTemplate(string Value)
   {
      public static implicit operator string(SqlTemplate c) => c.Value;
      public static implicit operator SqlTemplate(string s) => new (s);

      public override string ToString() => Value;
   }
}
