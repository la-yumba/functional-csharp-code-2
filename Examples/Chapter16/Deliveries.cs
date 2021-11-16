using System;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using System.Linq;

namespace Examples.Chapter16
{
   public class Deliveries
   {
      public static async Task Main_0()
      {
         await foreach (var line in ReadLines("warehouse.csv"))
         {
            Delivery r = Parse(line);
            UpdateDB(r);
         }
      }

      public static async Task Main_1()
         => await ReadDeliveries("warehouse.csv")
            .ForEachAsync(UpdateDB);

      public static async Task Main_2()
         => await ReadDeliveries("warehouse.csv")
            .ForEachAwaitAsync(UpdateDBAsync);

      public static async Task Main_3()
         => await ReadDeliveries("warehouse.csv")
            .GroupBy(r => r.ProductID)
            .SelectAwait(async grp => new Delivery
               (
                  ProductID: grp.Key,
                  Quantity: await grp.SumAsync(r => r.Quantity)
               ))
            .ForEachAwaitAsync(UpdateDBAsync);

      static IAsyncEnumerable<Delivery> ReadDeliveries(string path)
         => from line in ReadLines(path)
            select Parse(line);

      static IAsyncEnumerable<Delivery> ReadDeliveries_Rec(string dirPath)
         => from path in Directory.EnumerateFiles(dirPath, "*.csv").ToAsyncEnumerable()
            from line in ReadLines(path)
            select Parse(line);

      record Delivery(long ProductID, int Quantity);

      static Delivery Parse(string s)
      {
         string[] ss = s.Split(',');
         return new(long.Parse(ss[0]), int.Parse(ss[1]));
      }

      static void UpdateDB(Delivery r) => throw new NotImplementedException();
      static Task UpdateDBAsync(Delivery r) => throw new NotImplementedException();

      static async IAsyncEnumerable<string> ReadLines(string path)
      {
         using StreamReader reader = File.OpenText(path);
         while (!reader.EndOfStream)
            yield return await reader.ReadLineAsync();
      }
   }
}