using static System.Console;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using LaYumba.Functional;

using Rates = System.Collections.Immutable.ImmutableDictionary<string, decimal>;
using CurrencyCode = Boc.Domain.CurrencyCode;
using Decimal = LaYumba.Functional.Decimal;

namespace Examples.Chapter16
{
   public class CurrencyLookup_Stateless
   {
      public static void _main()
      {
         WriteLine("Enter a currency pair like 'EURUSD' to get a quote, or 'q' to quit");
         for (string input; (input = ReadLine().ToUpper()) != "Q";)
            WriteLine(FxApi.GetRate(input).Map(Decimal.ToString)
               .Recover(ex => ex.Message).Result); // what to do in case of failure
      }
   }

   static class RatesApi
   {
      static string UriFor(CurrencyCode baseCcy)
         => $"https://api.ratesapi.io/api/latest?base={baseCcy}";

      record Response(CurrencyCode Base, Rates Rates);

      static JsonSerializerOptions opts = new()
      { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

      public static decimal GetRate(string ccyPair)
      {
         WriteLine($"fetching rate...");

         var (baseCcy, quoteCcy) = ccyPair.SplitAt(3);
         var uri = $"https://api.ratesapi.io/api/latest?base={baseCcy}";
         Task<string> request = new HttpClient().GetStringAsync(uri);

         string body = request.Result;
         var response = JsonSerializer.Deserialize<Response>(body, opts);

         return response.Rates[quoteCcy];
      }

      public static async Task<decimal> GetRateAsync(string ccyPair)
      {
         WriteLine($"fetching rate...");

         var (baseCcy, quoteCcy) = ccyPair.SplitAt(3);
         var uri = $"https://api.ratesapi.io/api/latest?base={baseCcy}";

         Task<string> request = new HttpClient().GetStringAsync(uri);
         string body = await request;

         var response = JsonSerializer.Deserialize<Response>(body, opts);
         return response.Rates[quoteCcy];
      }

      public static Task<decimal> GetRateAsync_WithLinq1(string ccyPair)
         => GetRateAsync(ccyPair.SplitAt(3));

      public static Task<decimal> GetRateAsync((CurrencyCode Base, CurrencyCode Quote) pair)
         => from body in new HttpClient().GetStringAsync(UriFor(pair.Base))
            let response = JsonSerializer.Deserialize<Response>(body, opts)
            select response.Rates[pair.Quote];

      public static Task<decimal> GetRateAsync_WithStream
         ((CurrencyCode Base, CurrencyCode Quote) pair)
         => from stream in new HttpClient().GetStreamAsync(UriFor(pair.Base))
            from response in JsonSerializer.DeserializeAsync<Response>(stream, opts)
            select response.Rates[pair.Quote];

   }

   static class FxApi
   {
      public static Task<decimal> GetRate(string ccyPair) =>
         CurrencyLayer.GetRateAsync(ccyPair)
            .OrElse(() => RatesApi.GetRateAsync(ccyPair));
   }

   /// <summary>
   /// Note that:
   /// 1. you need to get an API key from https://currencylayer.com/ (free, registration required)
   /// 2. the free plan only allows you to query rates with USD as base currency
   /// </summary>
   public static class CurrencyLayer
   {
      static string key = "4772f4e46027c9047c9a2f7444c95c60";

      record Response(CurrencyCode Source, Rates Quotes);

      static JsonSerializerOptions opts = new()
      { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

      public static Task<decimal> GetRateAsync(string ccyPair) =>
         from body in new HttpClient().GetStringAsync(QueryFor(ccyPair))
         let response = JsonSerializer.Deserialize<Response>(body, opts)
         select response.Quotes[ccyPair];

      static string QueryFor(string pair)
         => $"http://www.apilayer.net/api/live?access_key={key}";
   }
}
