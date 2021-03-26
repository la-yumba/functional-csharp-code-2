using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples.Chapter9
{
   public class Employee { public string LastName { get; } }

   public static class EmployeeLookup
   {
      public static void Run()
      {
         ConnectionString conn = "my-database";

         SqlTemplate select = "SELECT * FROM EMPLOYEES"
            , sqlById = $"{select} WHERE ID = @Id"
            , sqlByName = $"{select} WHERE LASTNAME = @LastName";

         // queryById : object → IEnumerable<Employee>
         var queryById = conn.Retrieve<Employee>(sqlById);

         // queryByLastName : object → IEnumerable<Employee>
         var queryByLastName = conn.Retrieve<Employee>(sqlByName);

         // LookupEmployee : Guid → Option<Employee>
         Option<Employee> LookupEmployee(Guid id)
            => queryById(new { Id = id }).FirstOrDefault();

         // FindEmployeesByLastName : string → IEnumerable<Employee>
         IEnumerable<Employee> FindEmployeesByLastName(string lastName)
            => queryByLastName(new { LastName = lastName });
      }
   }
}
