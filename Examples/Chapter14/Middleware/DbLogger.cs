using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Examples.Chapter2.DbLogger;
using Microsoft.Extensions.Logging;
using Unit = System.ValueTuple;

namespace Examples.Chapter14
{
   public class LogMessage { }

   public class DbLogger_PyramidOfDoom
   {
      string connString;

      public void Log(LogMessage message)
         => Instrumentation.Time("CreateLog"
            , () => ConnectionHelper.Connect(connString
               , c => c.Execute("sp_create_log"
                  , message, commandType: CommandType.StoredProcedure)));
   }

   public class DbLogger
   {
      Middleware<SqlConnection> Connect;
      Func<string, Middleware<Unit>> Time;
      Func<string, Middleware<Unit>> Trace;

      public DbLogger(ConnectionString connString, ILogger log)
      {
         Connect = f => ConnectionHelper.Connect(connString, f);
         Time = op => f => Instrumentation.Time(log, op, f.ToNullary());
         Trace = op => f => Instrumentation.Trace(log, op, f.ToNullary());
      }

      Middleware<SqlConnection> BasicPipeline =>
         from _ in Time("InsertLog")
         from conn in Connect
         select conn;

      // demonstrates running a pipeline by directly passing a 
      // continuation; that is, without using Run
      // this will produce a dynamic value, which will be unsafely cast to int
      public int Dynamic_Log(LogMessage message) 
         => BasicPipeline(conn
            => conn.Execute("sp_create_log", message
               , commandType: CommandType.StoredProcedure));

      public void Log_1(LogMessage message) =>
         Connect(conn => conn.Execute("sp_create_log", message
            , commandType: CommandType.StoredProcedure));

      public void Log_2(LogMessage message) =>
         Connect
            .Map(conn => conn.Execute("sp_create_log", message
               , commandType: CommandType.StoredProcedure))
            .Run();

      public void Log(LogMessage message) => (
         from _ in Time("InsertLog")
         from conn in Connect
         select conn.Execute("sp_create_log", message
                            , commandType: CommandType.StoredProcedure)
      ).Run();

      public void DeleteOldLogs() => (
         from _ in Time("DeleteOldLogs")
         from conn in Connect
         select conn.Execute("DELETE [Logs] WHERE [Timestamp] < @upTo"
                            , new { upTo = 7.Days().Ago() })
      ).Run();

      public IEnumerable<LogMessage> GetLogs(DateTime since) => (
         from _    in Trace("GetLogs")
         from __   in Time("GetLogs")
         from conn in Connect
         select conn.Query<LogMessage>(@"SELECT * 
            FROM [Logs] WHERE [Timestamp] > @since", new { since = since })
      ).Run();
   }
}
