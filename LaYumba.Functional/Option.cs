using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Unit = System.ValueTuple;

namespace LaYumba.Functional
{
   using static F;

   public static partial class F
   {
      // wrap the given value into a Some
      public static Option<T> Some<T>([NotNull] T? value) // NotNull: `value` is guaranteed to never be null if the method returns without throwing an exception
         => new(value ?? throw new ArgumentNullException(nameof(value)));

      // the None value
      public static NoneType None => default;
   }

   // a NoneType can be implicitely converted to an Option<T> for any type T
   public struct NoneType {}

   public struct Option<T> : IEquatable<NoneType>, IEquatable<Option<T>>
   {
      readonly T? value;
      readonly bool isSome;
      bool isNone => !isSome;

      internal Option(T t) => (isSome, value) = (true, t);

      public static implicit operator Option<T>(NoneType _) => default;

      public static implicit operator Option<T>(T t)
         => t is null ? None : new Option<T>(t);

      public R Match<R>(Func<R> None, Func<T, R> Some)
         => isSome ? Some(value!) : None();

      public IEnumerable<T> AsEnumerable()
      {
         if (isSome) yield return value!;
      }

      public static bool operator true(Option<T> @this) => @this.isSome;
      public static bool operator false(Option<T> @this) => @this.isNone;
      public static Option<T> operator | (Option<T> l, Option<T> r) => l.isSome ? l : r;

      // equality operators

      public bool Equals(Option<T> other)
         => this.isSome == other.isSome
         && (this.isNone || this.value!.Equals(other.value));

      public bool Equals(NoneType _) => isNone;

      public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);
      public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

      public override bool Equals(object? other)
         => other is Option<T> option && this.Equals(option);

      public override int GetHashCode()
         => isNone ? 0 : value!.GetHashCode();

      public override string ToString() => isSome ? $"Some({value})" : "None";
   }

   public static class OptionExt
   {
      public static Option<R> Apply<T, R>
         (this Option<Func<T, R>> @this, Option<T> arg)
         => @this.Match(
            () => None,
            (func) => arg.Match(
               () => None,
               (val) => Some(func(val))));

      public static Option<Func<T2, R>> Apply<T1, T2, R>
         (this Option<Func<T1, T2, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.Curry), arg);

      public static Option<Func<T2, T3, R>> Apply<T1, T2, T3, R>
         (this Option<Func<T1, T2, T3, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>
         (this Option<Func<T1, T2, T3, T4, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>
         (this Option<Func<T1, T2, T3, T4, T5, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>
         (this Option<Func<T1, T2, T3, T4, T5, T6, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>
         (this Option<Func<T1, T2, T3, T4, T5, T6, T7, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, T5, T6, T7, T8, R>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, R>
         (this Option<Func<T1, T2, T3, T4, T5, T6, T7, T8, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<Func<T2, T3, T4, T5, T6, T7, T8, T9, R>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>
         (this Option<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> @this, Option<T1> arg)
         => Apply(@this.Map(F.CurryFirst), arg);

      public static Option<R> Bind<T, R>
         (this Option<T> optT, Func<T, Option<R>> f)
          => optT.Match(
             () => None,
             (t) => f(t));

      public static IEnumerable<R> Bind<T, R>
         (this Option<T> @this, Func<T, IEnumerable<R>> func)
          => @this.AsEnumerable().Bind(func);

      public static Option<Unit> ForEach<T>(this Option<T> @this, Action<T> action)
         => Map(@this, action.ToFunc());

      public static Option<R> Map<T, R>
         (this NoneType _, Func<T, R> f)
         => None;

      public static Option<R> Map<T, R>
         (this Option<T> optT, Func<T, R> f)
         => optT.Match(
            () => None,
            (t) => Some(f(t)));

      public static Option<Func<T2, R>> Map<T1, T2, R>
         (this Option<T1> @this, Func<T1, T2, R> func)
          => @this.Map(func.Curry());

      public static Option<Func<T2, T3, R>> Map<T1, T2, T3, R>
         (this Option<T1> @this, Func<T1, T2, T3, R> func)
          => @this.Map(func.CurryFirst());

      public static IEnumerable<Option<R>> Traverse<T, R>(this Option<T> @this
         , Func<T, IEnumerable<R>> func)
         => @this.Match(
            () => List((Option<R>)None),
            (t) => func(t).Map(r => Some(r)));

      // utilities

      public static Unit Match<T>(this Option<T> @this, Action None, Action<T> Some)
          => @this.Match(None.ToFunc(), Some.ToFunc());

      internal static bool IsSome<T>(this Option<T> @this)
         => @this.Match(
            () => false,
            (_) => true);

      internal static T ValueUnsafe<T>(this Option<T> @this)
         => @this.Match(
            () => { throw new InvalidOperationException(); },
            (t) => t);

      public static T GetOrElse<T>(this Option<T> opt, T defaultValue)
         => opt.Match( 
            () => defaultValue,
            (t) => t);

      public static T GetOrElse<T>(this Option<T> opt, Func<T> fallback)
         => opt.Match(
            () => fallback(),
            (t) => t);

      public static Task<T> GetOrElse<T>(this Option<T> opt, Func<Task<T>> fallback)
         => opt.Match(
            () => fallback(),
            (t) => Async(t));

      public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
         => left.Match( 
            () => right,
            (_) => left);

      public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
         => left.Match(
            () => right(), 
            (_) => left);

      public static Validation<T> ToValidation<T>(this Option<T> opt, Error error)
         => opt.Match(
            () => Invalid(error),
            (t) => Valid(t));

      public static Validation<T> ToValidation<T>(this Option<T> opt, Func<Error> error)
         => opt.Match(
            () => Invalid(error()),
            (t) => Valid(t));

      // LINQ

      public static Option<R> Select<T, R>(this Option<T> @this, Func<T, R> func)
         => @this.Map(func);

      public static Option<T> Where<T>
         (this Option<T> optT, Func<T, bool> predicate)
         => optT.Match(
            () => None,
            (t) => predicate(t) ? optT : None);

      public static Option<RR> SelectMany<T, R, RR>
         (this Option<T> opt, Func<T, Option<R>> bind, Func<T, R, RR> project)
         => opt.Match(
            () => None,
            (t) => bind(t).Match(
               () => None,
               (r) => Some(project(t, r))));
   }
}
