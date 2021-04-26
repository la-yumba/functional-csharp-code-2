using System;
using System.Text.Json;

using LaYumba.Functional;
using static LaYumba.Functional.F;

using NUnit.Framework;

namespace Examples.Chapter14
{
   public class TryTests
   {
      Exceptional<Uri> Boilerplate_CreateUri(string uri)
      {
         try { return new Uri(uri); }
         catch (Exception ex) { return ex; }
      }

      record Website(string Name, string Uri);

      public static void Run()
      {
         var json = @"{""Name"":""Github"",
            ""Uri"":""http://github.com""}";
         Console.WriteLine(ExtractUri_Simple(json));
      }

      public static Uri ExtractUri_Simple(string json)
      {
         var deserialized = JsonSerializer.Deserialize<Website>(json);
         return new Uri(deserialized.Uri);
      }

      Try<Uri> CreateUri(string uri) => () => new Uri(uri);

      Try<T> Parse<T>(string s) => () => JsonSerializer.Deserialize<T>(s);

      Try<Uri> ExtractUri(string json) =>
         from website in Parse<Website>(json)
         from uri in CreateUri(website.Uri)
         select uri;

      [TestCase(@"{""Name"":""Github"", ""Uri"":""http://github.com""}"
         , ExpectedResult = "Ok")]
      [TestCase(@"{""Name"":""Github"", ""Uri"":""rubbish""}"
         , ExpectedResult = "Invalid URI")]
      [TestCase("{}"
         , ExpectedResult = "Value cannot be null")]
      [TestCase("blah!"
         , ExpectedResult = "'b' is an invalid start of a value")]
      public string SuccessfulTry(string json)
         => ExtractUri(json)
            .Run()
            .Match
            (
               ex => ex.Message.Split(new char[] { '.', ':' })[0],
               _ => "Ok"
            );
   }
}
