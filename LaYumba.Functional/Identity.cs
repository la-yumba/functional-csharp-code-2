using System;

namespace LaYumba.Functional
{
   // The book describes `Func<T>` as a monad over `T`.

   // This is sometimes referred to as "the identity monad", hence here I
   // define the `Identity<T>` delegate to capture this, and the `Identity`
   // function which is its Return function, lifting a `T` into an `Identity<T>`
   // (This is of limited practical interest, hence not discussed in the book itself.)

   public static partial class F
   {
      public static Identity<T> Identity<T>(T value) => () => value;
   }
   
   // () -> T (aka. Identity)
   public static class FuncTExt
   {      
      public static Func<R> Map<T, R>
         (this Func<T> f, Func<T, R> g) 
         => () => g(f());

      public static Func<R> Bind<T, R>
         (this Func<T> f, Func<T, Func<R>> g) 
         => () => g(f())();

      // LINQ

      public static Func<R> Select<T, R>(this Func<T> @this
         , Func<T, R> func) => @this.Map(func);

      public static Func<P> SelectMany<T, R, P>(this Func<T> @this
         , Func<T, Func<R>> bind, Func<T, R, P> project)
         => () =>
         {
            T t = @this();
            R r = bind(t)();
            return project(t, r);
         };
   }

   // same, with custom delegate

   public delegate T Identity<T>();

   public static class IdentityExt
   {
      public static Identity<R> Map<T, R>(this Identity<T> @this
         , Func<T, R> func) => () => func(@this());

      public static Identity<R> Bind<T, R>(this Identity<T> @this
         , Func<T, Identity<R>> func) => () => func(@this())();
   }
}
