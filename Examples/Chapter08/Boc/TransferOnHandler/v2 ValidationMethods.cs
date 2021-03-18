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
      DateTime now;
      Regex bicRegex = new Regex("[A-Z]{11}");

      Either<Error, Unit> Handle(MakeTransfer request)
         => Right(request)
            .Bind(ValidateBic)
            .Bind(ValidateDate)
            .Bind(Save);

      Either<Error, MakeTransfer> ValidateBic(MakeTransfer request)
      {
         if (!bicRegex.IsMatch(request.Bic))
            return Errors.InvalidBic;
         else return request;
      }

      Either<Error, MakeTransfer> ValidateDate(MakeTransfer request)
      {
         if (request.Date.Date <= now.Date)
            return Errors.TransferDateIsPast;
         else return request;
      }

      Either<Error, Unit> Save(MakeTransfer request)
      { throw new NotImplementedException(); }
   }
}
