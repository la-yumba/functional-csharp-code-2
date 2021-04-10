using System;

namespace LaYumba.Functional.Data.BinaryTree
{
   public abstract record Tree<T>;
   internal record Leaf<T>(T Value) : Tree<T>;
   internal record Branch<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;

   public static class Tree
   {
      public static R Match<T, R>(this Tree<T> tree, Func<T, R> Leaf, Func<Tree<T>, Tree<T>, R> Branch)
         => tree switch
         {
            Leaf<T>(T val) => Leaf(val),
            Branch<T>(var l, var r) => Branch(l, r),
            _ => throw new ArgumentException("{tree} is not a valid tree")
         };

      public static Tree<T> Leaf<T>(T Value) => new Leaf<T>(Value);

      public static Tree<T> Branch<T>(Tree<T> Left, Tree<T> Right)
         => new Branch<T>(Left, Right);

      public static Tree<R> Map<T, R>(this Tree<T> @this, Func<T, R> f)
         => @this.Match(
            Leaf: t => Leaf(f(t)),
            Branch: (left, right) => Branch
               (
                  Left: left.Map(f),
                  Right: right.Map(f)
               )
         );

      public static Tree<T> Insert<T>(this Tree<T> @this, T value)
         => @this.Match(
            Leaf: t => Branch(Leaf(t), Leaf(value)),
            Branch: (l, r) => Branch(l, r.Insert(value)));

      public static T Aggregate<T>(this Tree<T> tree, Func<T, T, T> f)
         => tree.Match(
            Leaf: t => t,
            Branch: (l, r) => f(l.Aggregate(f), r.Aggregate(f)));

      public static Acc Aggregate<T, Acc>(this Tree<T> tree, Acc acc, Func<Acc, T, Acc> f)
         => tree.Match(
            Leaf: t => f(acc, t),
            Branch: (l, r) =>
            {
               var leftAcc = l.Aggregate(acc, f);
               return r.Aggregate(leftAcc, f);
            });

      public static Tree<R> Run<T, R>(this Coyo<Tree<T>, R> @this)
         => @this.Value.Map(t => @this.Func(t));
   }
}
