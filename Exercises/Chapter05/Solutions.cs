using System;
using System.Collections.Generic;
using LaYumba.Functional;
using System.Text.RegularExpressions;

namespace Exercises.Chapter5
{
   using static F;

   public static class Solutions
   {
      // 1 Write a generic function that takes a string and parses it as a value of an enum. It
      // should be usable as follows:

      // Enum.Parse<DayOfWeek>("Friday") // => Some(DayOfWeek.Friday)
      // Enum.Parse<DayOfWeek>("Freeday") // => None

      // solution: see LaYumba.Functional.Enum.Parse<T>


      // 2 Write a Lookup function that will take an IEnumerable and a predicate, and
      // return the first element in the IEnumerable that matches the predicate, or None
      // if no matching element is found. Write its signature in arrow notation:

      // bool isOdd(int i) => i % 2 == 1;
      // new List<int>().Lookup(isOdd) // => None
      // new List<int> { 1 }.Lookup(isOdd) // => Some(1)

      // Lookup : IEnumerable<T> -> (T -> bool) -> Option<T>
      public static Option<T> Lookup<T>(this IEnumerable<T> ts, Func<T, bool> pred)
      {
         foreach (T t in ts) if (pred(t)) return Some(t);
         return None;
      }

      // 3 Write a type Email that wraps an underlying string, enforcing that it’s in a valid
      // format. Ensure that you include the following:
      // - A smart constructor
      // - Implicit conversion to string, so that it can easily be used with the typical API
      // for sending emails

      public class Email
      {
         static readonly Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

         private string Value { get; }

         private Email(string value) => Value = value;

         public static Option<Email> Create(string s)
            => regex.IsMatch(s) 
               ? Some(new Email(s)) 
               : None;

         public static implicit operator string(Email e)
            => e.Value;
      }
   }
}
