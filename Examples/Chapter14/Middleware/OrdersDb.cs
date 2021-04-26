using System;
using System.Data.SqlClient;

using Dapper;

using ConnectionHelper = Examples.Chapter2.DbLogger.ConnectionHelper;

namespace Examples.Chapter14
{
   public static class OrdersDb
   {
      static Middleware<SqlConnection> Connect(ConnectionString connString)
         => f => ConnectionHelper.Connect(connString, f);

      static Middleware<SqlTransaction> Transact(SqlConnection conn)
         => f => ConnectionHelper.Transact(conn, f);

      public static Func<Guid, int> DeleteOrder(ConnectionString connString)
         => (Guid id) =>
      {
         SqlTemplate deleteLinesSql = "DELETE OrderLines WHERE OrderId = @Id";
         SqlTemplate deleteOrderSql = "DELETE Orders WHERE Id = @Id";

         object param = new { Id = id };

         Middleware<int> deleteOrder =
            from conn in Connect(connString)
            from tran in Transact(conn)
            select conn.Execute(deleteLinesSql, param, tran)
                 + conn.Execute(deleteOrderSql, param, tran);

         return deleteOrder.Run();
      };
   }
}
