using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Examples.AppendixA.Immutable
{
   // code reproduced from LaYumba.Functional v1

   public static class Immutable
   {
      public static T With<T>(this T source, string propertyName, object newValue)
         where T : class
      {
         T @new = source.ShallowCopy();

         typeof(T).GetBackingField(propertyName)
            .SetValue(@new, newValue);

         return @new;
      }

      public static T With<T, P>(this T source, Expression<Func<T, P>> exp, object newValue)
         where T : class
         => source.With(exp.MemberName(), newValue);

      static string MemberName<T, P>(this Expression<Func<T, P>> e)
         => ((MemberExpression)e.Body).Member.Name;

#pragma warning disable CS8600, CS8603, CS8602
      static T ShallowCopy<T>(this T source)
         => (T)source.GetType().GetTypeInfo().GetMethod("MemberwiseClone"
               , BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(source, null);
#pragma warning restore CS8602, CS8603, CS8600

      static string BackingFieldName(string propertyName)
         => string.Format("<{0}>k__BackingField", propertyName);
      
      static FieldInfo GetBackingField(this Type t, string propertyName)
         => t.GetTypeInfo().GetField(BackingFieldName(propertyName)
            , BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new ArgumentException($"Type '{t.FullName}' does not expose a property called '{propertyName}'", nameof(propertyName));
   }
}
