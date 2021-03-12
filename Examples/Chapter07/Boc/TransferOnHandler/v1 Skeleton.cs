using Boc.Commands;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using System;
using Unit = System.ValueTuple;

namespace Boc.EitherImpl.Services.Skeleton
{
   class MakeTransferController_Skeleton : ControllerBase
   {
      [HttpPost, Route("transfers/book/skeleton")]
      public void MakeTransfer([FromBody] MakeTransfer request)
         => Handle(request);

      Either<Error, Unit> Handle(MakeTransfer request)
         => Validate(request)
            .Bind(Save);

      Either<Error, MakeTransfer> Validate(MakeTransfer request)
      { throw new NotImplementedException(); }

      Either<Error, Unit> Save(MakeTransfer request)
      { throw new NotImplementedException(); }
   }
}