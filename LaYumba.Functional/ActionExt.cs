using System;
using System.Threading.Tasks;
using Unit = System.ValueTuple;

namespace LaYumba.Functional
{
   using static F;

   public static class ActionExt
   {
      public static Func<Unit> ToFunc(this Action action)
          => () => { action(); return default; };

      public static Func<T, Unit> ToFunc<T>(this Action<T> action)
          => t => { action(t); return default; };

      public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> action)
          => (T1 t1, T2 t2) => { action(t1, t2); return default; };

      public static Func<Task<Unit>> ToFunc(this Func<Task> f)
         => async () => { await f(); return Unit(); };

      public static Func<T, Task<Unit>> ToFunc<T>(this Func<T, Task> f)
         => async (t) => { await f(t); return Unit(); };

      public static Func<T1, T2, Task<Unit>> ToFunc<T1, T2>(this Func<T1, T2, Task> f)
         => async (t1, t2) => { await f(t1, t2); return Unit(); };
   }
}
