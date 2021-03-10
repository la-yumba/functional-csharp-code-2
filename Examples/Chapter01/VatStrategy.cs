using System;
namespace Examples.Chapter01
{
   record Product(string Name, double Price, bool IsFood);

   record Order(Product Product, int Quantity)
   {
      public double NetPrice => Product.Price * Quantity;
   }

   record Address(string Country);
   record UsAddress(string State) : Address("us");

   public static class VatStrategy
   {
      static double Vat(Address address, Order order)
         => address switch
         {
            UsAddress(var state) => Vat(RateByState(state), order),
            Address("de") => DeVat(order),
            Address(var country) => Vat(RateByCountry(country), order),
         };

      static double RateByCountry(string country)
         => country switch
         {
            "it" => 0.22,
            "jp" => 0.08,
            _ => throw new ArgumentException($"Missing rate for {country}")
         };

      static double Vat(double rate, Order order)
         => order.NetPrice * rate;

      static double RateByState(string state)
         => state switch
         {
            "ca" => 0.1,
            "ma" => 0.0625,
            "ny" => 0.085,
            _ => throw new ArgumentException($"Missing rate for {state}")
         };

      static double DeVat(Order order)
         => order.NetPrice * (order.Product.IsFood ? 0.08 : 0.2);
   }
}
