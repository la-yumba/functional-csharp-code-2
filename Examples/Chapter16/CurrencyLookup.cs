using static System.Console;
using System.Linq;
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
      // get your own key if my free trial has expired
      const string ApiKey = "1a2419e081f5940872d5700f";

      public static decimal GetRate(string ccyPair)
      {
         WriteLine($"fetching rate...");

         Task<string> request = new HttpClient()
            .GetStringAsync(UriFor(ccyPair));

         string body = request.Result;

         var response = JsonSerializer.Deserialize<Response>(body, opts);
         return response.ConversionRate;
      }

      static string UriFor(string ccyPair)
      {
         var (baseCcy, quoteCcy) = ccyPair.SplitAt(3);
         return $"https://v6.exchangerate-api.com/v6/{ApiKey}" +
            $"/pair/{baseCcy}/{quoteCcy}";
      }

      record Response(decimal ConversionRate);

      static readonly JsonSerializerOptions opts = new()
      { PropertyNamingPolicy = new SnakeCaseNamingPolicy() };

      public static async Task<decimal> GetRateAsync(string ccyPair)
      {
         WriteLine($"fetching rate...");

         Task<string> request = new HttpClient()
            .GetStringAsync(UriFor(ccyPair));

         string body = await request;

         var response = JsonSerializer.Deserialize<Response>(body, opts);
         return response.ConversionRate;
      }

      public static Task<decimal> GetRateAsync_WithLinq1(string ccyPair)
         => from body in new HttpClient().GetStringAsync(UriFor(ccyPair))
            let response = JsonSerializer.Deserialize<Response>(body, opts)
            select response.ConversionRate;

      public static Task<decimal> GetRateAsync_WithStream(string ccyPair)
         => from stream in new HttpClient().GetStreamAsync(UriFor(ccyPair))
            from response in JsonSerializer.DeserializeAsync<Response>(stream, opts)
            select response.ConversionRate;
   }

   public class SnakeCaseNamingPolicy : JsonNamingPolicy
   {
      public override string ConvertName(string name) => ToSnakeCase(name);

      public static string ToSnakeCase(string str) => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
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
