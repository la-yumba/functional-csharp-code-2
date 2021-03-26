using System;
using System.Collections.Generic;
using Dapper;
using Examples.Chapter2.DbLogger;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Unit = System.ValueTuple;

namespace Examples
{
   using static ConnectionHelper;
   
   public static class ConnectionStringExt
   {
      public static Func<object, IEnumerable<T>>Retrieve<T>
         (this ConnectionString connString, SqlTemplate sql)
         => param
         => Connect(connString, conn => conn.Query<T>(sql, param));

      public static Func<object, Exceptional<Unit>>TryExecute
         (this ConnectionString connString, SqlTemplate sql)
         => param =>
      {
         try { Connect(connString, conn => conn.Execute(sql, param)); }
         catch (Exception ex) { return ex; }
         return Unit();
      };
   }
}
