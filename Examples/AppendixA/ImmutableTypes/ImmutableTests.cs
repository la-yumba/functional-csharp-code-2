using NUnit.Framework;

namespace Examples.AppendixA.Immutable
{
   class A
   {
      public int B { get; }
      public string C { get; }
      public A(int b, string c) { B = b; C = c; }
   }

   
   public class Immutable_With_PropertyName
   {
      A original = new A(123, "hello");

      [Test]
      public void ItChangesTheDesiredProperty()
      {
         var @new = original.With("B", 777);

         Assert.AreEqual(777, @new.B); // updated
         Assert.AreEqual("hello", @new.C); // same as original
      }

      [Test]
      public void ItDoesNotChangesTheOriginal()
      {
         var @new = original.With("B", 777);

         Assert.AreEqual(123, original.B);
         Assert.AreEqual("hello", original.C);
      }
   }

   
   public class Immutable_With_PropertyExpression
   {
      A original = new A(123, "hello");

      [Test]
      public void ItChangesTheDesiredProperty()
      {
         var @new = original.With(a => a.C, "hi");

         Assert.AreEqual(123, original.B);
         Assert.AreEqual("hello", original.C);

         Assert.AreEqual(123, @new.B);
         Assert.AreEqual("hi", @new.C);
      }

      [Test]
      public void YouCanChainWith()
      {
         var @new = original
            .With(a => a.B, 345)
            .With(a => a.C, "howdy");

         Assert.AreEqual(345, @new.B);
         Assert.AreEqual("howdy", @new.C);      }
   }
}
