using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;

namespace Exercises.Chapter6
{
   static class Exercises
   {
      // 1 Implement Map for ISet<T> and IDictionary<K, T>. (Tip: start by writing down
      // the signature in arrow notation.)

      // 2 Implement Map for Option and IEnumerable in terms of Bind and Return.

      // 3 Use Bind and an Option-returning Lookup function (such as the one we defined
      // in chapter 3) to implement GetWorkPermit, shown below. 

      // Then enrich the implementation so that `GetWorkPermit`
      // returns `None` if the work permit has expired.

      static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> people, string employeeId)
      {
         throw new NotImplementedException();
      }

      // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
      // employees who have left should be included).

      static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
      {
         // your implementation here...
         throw new NotImplementedException();
      }
   }

   public record Employee
   (
      string Id,
      Option<WorkPermit> WorkPermit,

      DateTime JoinedOn,
      Option<DateTime> LeftOn
   );

   public record WorkPermit
   (
      string Number,
      DateTime Expiry
   );
}