using Boc.Commands;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using System;
using Unit = System.ValueTuple;

namespace Boc.Api
{
   public class Chapter6_MakeTransferController : ControllerBase
   {
      [HttpPost, Route("api/Chapters6/transfers/future/restful")]
      public IActionResult MakeTransfer_v1([FromBody] MakeTransfer request)
         => Handle(request).Match<IActionResult>(
            Right: _ => Ok(),
            Left: BadRequest);

      [HttpPost, Route("api/Chapters6/transfers/future/resultDto")]
      public ResultDto<Unit> MakeTransfer_v2([FromBody] MakeTransfer request)
         => Handle(request).ToResult();

      Either<Error, Unit> Handle(MakeTransfer request) { throw new NotImplementedException();  }
   }
}
