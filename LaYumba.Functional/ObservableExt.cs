using System;
using System.Threading.Tasks;
using LaYumba.Functional;
using static LaYumba.Functional.F;

using System.Reactive.Linq;

namespace LaYumba.Functional
{
   public static class ObservableExt
   {
      // Safely performs a Task-returning function for each t in ts,
      // and returns a stream of results for the completed tasks, 
      // and a stream of exceptions
      public static (IObservable<R> Completed, IObservable<Exception> Faulted) 
         Safely<T, R>(this IObservable<T> ts, Func<T, Task<R>> f)
         => ts
            .SelectMany(t =>
               Observable.FromAsync(() =>
                  f(t).Map(
                     Faulted: ex => ex,
                     Completed: r => Exceptional(r))))
            .Partition();

      public static (IObservable<T> Successes, IObservable<Exception> Exceptions) 
         Partition<T>(this IObservable<Exceptional<T>> excTs)
      {
         bool IsSuccess(Exceptional<T> ex) 
            => ex.Match(_ => false, _ => true);

         T ExtractValue(Exceptional<T> ex)
            => ex.Match(_ => throw new InvalidOperationException(), t => t);

         Exception ExtractException(Exceptional<T> ex)
            => ex.Match(exc => exc, _ => throw new InvalidOperationException());

         var (ts, errs) = excTs.Partition(IsSuccess);
         return
         (
            Successes: ts.Select(ExtractValue),
            Exceptions: errs.Select(ExtractException)
         );
      }

      public static (IObservable<T> Passed, IObservable<T> Failed) Partition<T>
      (
         this IObservable<T> source,
         Func<T, bool> predicate
      )
      =>
      (
         Passed: source.Where(predicate),
         Failed: source.Where(predicate.Negate())
      );

      public static (IObservable<RTrue> Passed, IObservable<RFalse> Failed) Partition<T, RTrue, RFalse>
         (this IObservable<T> source
         , Func<T, bool> If
         , Func<T, RTrue> Then
         , Func<T, RFalse> Else)
         => (source.Where(t => If(t)).Select(Then)
            , source.Where(t => !If(t)).Select(Else));
      
      public static IObservable<(T Previous, T Current)> 
         PairWithPrevious<T>(this IObservable<T> source)
         => source
            .Scan((Previous: default(T), Current: default(T))
               , (prev, current) => (prev.Current, current))
            .Skip(1);
   }
}
