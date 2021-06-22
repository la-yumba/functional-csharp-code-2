using Boc.Commands;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;
using Unit = System.ValueTuple;

namespace Boc.EitherImpl.Services.ValidationMethods
{
   using Domain;
   using static F;

   class MakeTransferController_Skeleton : ControllerBase
   {
      [HttpPost, Route("transfers/book")]
      public void MakeTransfer([FromBody] MakeTransfer request)
         => Handle(request);

      DateTime now;
      Regex bicRegex = new Regex("[A-Z]{11}");

      Either<Error, Unit> Handle(MakeTransfer transfer)
         => Right(transfer)
            .Bind(ValidateBic)
            .Bind(ValidateDate)
            .Bind(Save);

      Either<Error, MakeTransfer> ValidateBic(MakeTransfer transfer)
         => bicRegex.IsMatch(transfer.Bic)
            ? transfer
            : Errors.InvalidBic;

      Either<Error, MakeTransfer> ValidateDate(MakeTransfer transfer)
         => transfer.Date.Date > now.Date
            ? transfer
            : Errors.TransferDateIsPast;

      Either<Error, Unit> Save(MakeTransfer request)
         => throw new NotImplementedException();
    }
}
