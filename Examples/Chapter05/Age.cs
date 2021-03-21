using LaYumba.Functional;

namespace Examples.Chapter5
{
   using static F;

   public struct Age
   {
      public int Value { get; }

      // smart constructor
      public static Option<Age> Create(int age)
         => IsValid(age) ? Some(new Age(age)) : None;

      private Age(int value)
         => Value = value;

      private static bool IsValid(int age)
         => 0 <= age && age < 120;

      public static bool operator <(Age l, Age r) => l.Value < r.Value;
      public static bool operator >(Age l, Age r) => l.Value > r.Value;

      public static bool operator <(Age l, int r) => l < new Age(r);
      public static bool operator >(Age l, int r) => l > new Age(r);

      public override string ToString() => Value.ToString();
   }
}
