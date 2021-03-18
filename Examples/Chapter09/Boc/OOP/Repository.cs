using System;
using LaYumba.Functional;
using Boc.Commands;
using Unit = System.ValueTuple;

namespace Boc.Chapter9.OOP
{
   using static F;
   using Examples;

   public class MakeTransferRepository : IRepository<MakeTransfer>
   {
      ConnectionString conn;

      public MakeTransferRepository(ConnectionString conn)
      {
         this.conn = conn;
      }

      public Option<MakeTransfer> Lookup(Guid id)
      { throw new NotImplementedException("Illustrates violating interface segregation"); }

      public Exceptional<Unit> Save(MakeTransfer transfer)
      {
         try { conn.Execute("INSERT ...", transfer); }
         catch (Exception ex) { return ex; }
         return Unit();
      }
   }  
}
