using System;

namespace Examples
{
   public enum Risk { Low, Medium, High }
}

namespace Examples.Chapter4
{
   public static class RiskCalculator
   {
      // dynamic type (anything goes)
      // e.g.: CalculateRiskProfile_Dynamic("hello");
      // => compiles, but causes runtime exception
      static Risk CalculateRiskProfile_Dynamic(dynamic age)
         => (age < 60) ? Risk.Low : Risk.Medium;

      // primitive type (any int goes)
      static Risk CalculateRiskProfile_Int(int age)
         => (age < 60) ? Risk.Low : Risk.Medium;

      // primitive type with ad hoc validation
      static Risk CalculateRiskProfile_Throws(int age)
      {
         if (age < 0 || 120 <= age)
            throw new ArgumentException($"{age} is not a valid age");

         return (age < 60) ? Risk.Low : Risk.Medium;
      }

      // dedicated type
      public static Risk CalculateRiskProfile(Age age)
         => (age < 60) ? Risk.Low : Risk.Medium;
   }

   public struct Age
   {
      private int Value { get; }

      private Age(int value)
      {
         if (!IsValid(value))
            throw new ArgumentException($"{value} is not a valid age");

         Value = value;
      }

      private static bool IsValid(int age)
         => 0 <= age && age < 120;

      public static bool operator <(Age l, Age r) => l.Value < r.Value;
      public static bool operator >(Age l, Age r) => l.Value > r.Value;

      public static bool operator <(Age l, int r) => l < new Age(r);
      public static bool operator >(Age l, int r) => l > new Age(r);

      public override string ToString() => Value.ToString();
   }

   readonly record struct HealthData
   (
      Age Age,
      Gender Gender
   );
}
