using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Examples.Chapter2
{
   class Names
   {
      Comparison<string> caseInsensitive = (x, y) => x.ToUpper().CompareTo(y.ToUpper());

      public void Sort(List<string> names) => names.Sort(caseInsensitive);
   }

   class Numbers
   {
      static Comparison<int> alphabetically = (l, r)
         => l.ToString().CompareTo(r.ToString());

      static Comparison<int> alphabetically_anonymous_method = delegate (int l, int r)
      {
         return l.ToString().CompareTo(r.ToString());
      };

      [Test]
      public static void Run()
      {
         // => [3, 6, 9, 12, 15, 18, 21, 24, 27, 30]
         var list = Enumerable.Range(1, 10).Select(i => i * 3).ToList();

         list.Sort(alphabetically);
         Assert.AreEqual(new List<int>() { 12, 15, 18, 21, 24, 27, 3, 30, 6, 9 }
            , list);
      }
   }

   class Names_Lambda
   {
      public void Sort(List<string> names)
         => names.Sort((x, y) => x.ToUpper().CompareTo(y.ToUpper()));
   }

   public class Employee { public string LastName { get; } }

   public class Lambda_Closure
   {
      private List<Employee> employees;

      public IEnumerable<Employee> FindByName(string name)
         => employees.Where(e => e.LastName.StartsWith(name));
   }

   class Cache<T> where T : class
   {
      public T Get(Guid id, Func<T> onMiss) 
         => Get(id) ?? onMiss();
      
      T Get(Guid id)
      {
         throw new NotImplementedException();
      }
   }
}
