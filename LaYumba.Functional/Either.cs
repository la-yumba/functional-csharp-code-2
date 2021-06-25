using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;

namespace LaYumba.Functional
{
   using static F;

   public static partial class F
   {
      public static Either.Left<L> Left<L>(L l) => new Either.Left<L>(l);
      public static Either.Right<R> Right<R>(R r) => new Either.Right<R>(r);
   }

   public struct Either<L, R>
   {
      private L? Left { get; }
      private R? Right { get; }

      private bool IsRight { get; }
      private bool IsLeft => !IsRight;

      internal Either(L left)
         => (IsRight, Left, Right)
         = (false, left ?? throw new ArgumentNullException(nameof(left)), default);

      internal Either(R right)
         => (IsRight, Left, Right)
         = (true, default, right ?? throw new ArgumentNullException(nameof(right)));

      public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
      public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);

      public static implicit operator Either<L, R>(Either.Left<L> left) => new Either<L, R>(left.Value);
      public static implicit operator Either<L, R>(Either.Right<R> right) => new Either<L, R>(right.Value);

      public TR Match<TR>(Func<L, TR> Left, Func<R, TR> Right)
         => IsLeft ? Left(this.Left!) : Right(this.Right!);

      public Unit Match(Action<L> Left, Action<R> Right)
         => Match(Left.ToFunc(), Right.ToFunc());

      public IEnumerator<R> AsEnumerable()
      {
         if (IsRight) yield return Right!;
      }

      public override string ToString() => Match(l => $"Left({l})", r => $"Right({r})");
   }

   public static class Either
   {
      public struct Left<L>
      {
         internal L Value { get; }
         internal Left(L value) { Value = value; }

         public override string ToString() => $"Left({Value})";
      }

      public struct Right<R>
      {
         internal R Value { get; }
         internal Right(R value) { Value = value; }

         public override string ToString() => $"Right({Value})";

         public Right<RR> Map<L, RR>(Func<R, RR> f) => Right(f(Value));
         public Either<L, RR> Bind<L, RR>(Func<R, Either<L, RR>> f) => f(Value);
      }
   }

   public static class EitherExt
   {
      public static Either<L, RR> Map<L, R, RR>
         (this Either<L, R> @this, Func<R, RR> f)
         => @this.Match<Either<L, RR>>(
            l => Left(l),
            r => Right(f(r)));

      public static Either<LL, RR> Map<L, LL, R, RR>
         (this Either<L, R> @this, Func<L, LL> Left, Func<R, RR> Right)
         => @this.Match<Either<LL, RR>>
            (
               l => F.Left(Left(l)),
               r => F.Right(Right(r))
            );

      public static Either<L, Unit> ForEach<L, R>
         (this Either<L, R> @this, Action<R> act)
         => Map(@this, act.ToFunc());

      public static Either<L, RR> Bind<L, R, RR>
         (this Either<L, R> @this, Func<R, Either<L, RR>> f)
         => @this.Match(
            l => Left(l),
            r => f(r));


      // LINQ

      public static Either<L, R> Select<L, T, R>(this Either<L, T> @this
         , Func<T, R> map) => @this.Map(map);


      public static Either<L, RR> SelectMany<L, T, R, RR>(this Either<L, T> @this
         , Func<T, Either<L, R>> bind, Func<T, R, RR> project)
         => @this.Match(
            Left: l => Left(l),
            Right: t =>
               bind(t).Match<Either<L, RR>>(
                  Left: l => Left(l),
                  Right: r => project(t, r)));
   }
}
