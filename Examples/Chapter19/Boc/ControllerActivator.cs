using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using Boc.Commands;
using LaYumba.Functional;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boc.Chapter13.Domain;
using Unit = System.ValueTuple;
using Boc.Domain.Events;

namespace Boc.Chapter19
{
   public class ControllerActivator : IControllerActivator
   {
      Lazy<AccountRegistry> accountRegistry;

      Func<Guid, Task<IEnumerable<Event>>> loadEvents;
      Func<Event, Task<Unit>> saveAndPublish;
      Func<MakeTransfer, Validation<MakeTransfer>> validate;

      public ControllerActivator()
      {
         accountRegistry = new Lazy<AccountRegistry>(() =>
            new AccountRegistry
               ( loadAccount: id => loadEvents(id).Map(Account.From)
               , saveAndPublish: saveAndPublish));
      }

      public object Create(ControllerContext context)
      {
         var type = context.ActionDescriptor.ControllerTypeInfo.AsType();
         if (type.Equals(typeof(MakeTransferController)))
            return new MakeTransferController(
               getAccount: accountRegistry.Value.Lookup, 
               validate: validate);

         throw new InvalidOperationException("Unexpected controller type");
      }

      public void Release(ControllerContext context, object controller)
      {
         var disposable = controller as IDisposable;
         if (disposable != null) disposable.Dispose();
      }
   }
}
