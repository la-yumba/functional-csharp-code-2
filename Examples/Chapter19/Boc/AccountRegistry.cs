using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Threading.Tasks;
using Boc.Chapter13.Domain;
using Boc.Domain.Events;
using Unit = System.ValueTuple;
using System.Collections.Immutable;

namespace Boc.Chapter19
{
   using AccountsCache = ImmutableDictionary<Guid, AccountProcess>;

   public class AccountRegistry_Naive
   {
      Agent<Guid, Option<AccountProcess>> agent;

      public AccountRegistry_Naive(Func<Guid, Task<Option<AccountState>>> loadAccount
         , Func<Event, Task<Unit>> saveAndPublish)
         => agent = Agent.Start
         (
            initialState: AccountsCache.Empty,
            process: async (AccountsCache cache, Guid id) =>
            {
               if (cache.TryGetValue(id, out AccountProcess account))
                  return (cache, Some(account));

               var optAccount = await loadAccount(id);

               return optAccount.Map(accState =>
               {
                  AccountProcess account = new(accState, saveAndPublish);
                  return (cache.Add(id, account), Some(account));
               })
               .GetOrElse(() => (cache, None));
            }
         );
      
      public Task<Option<AccountProcess>> Lookup(Guid id)
         => agent.Tell(id);
   }

   public class AccountRegistry
   {
      Agent<Msg, Option<AccountProcess>> agent;
      Func<Guid, Task<Option<AccountState>>> loadAccount;

      abstract record Msg(Guid Id);
      record LookupMsg(Guid Id) : Msg(Id);
      record RegisterMsg(Guid Id, AccountState AccountState) : Msg(Id);

      public AccountRegistry
      (
         Func<Guid, Task<Option<AccountState>>> loadAccount,
         Func<Event, Task<Unit>> saveAndPublish
      )
      {
         this.loadAccount = loadAccount;

         this.agent = Agent.Start
         (
            initialState: AccountsCache.Empty,
            process: (AccountsCache cache, Msg msg) => msg switch
            {
               LookupMsg m => (cache, cache.Lookup(m.Id)),

               RegisterMsg m => cache.Lookup(m.Id).Match
               (
                  Some: acc => (cache, Some(acc)),
                  None: () =>
                  {
                     AccountProcess account = new(m.AccountState, saveAndPublish);
                     return (cache.Add(m.Id, account), Some(account));
                  }
               )
            }
         );
      }

      public Task<Option<AccountProcess>> Lookup(Guid id)
         => agent
         .Tell(new LookupMsg(id))
         .OrElse(() => 
            from state in loadAccount(id) // loading the state is done in the calling thread
            from account in agent.Tell(new RegisterMsg(id, state))
            select account);
   }
}
