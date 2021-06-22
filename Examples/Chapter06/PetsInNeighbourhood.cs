using System.Collections.Generic;
using LaYumba.Functional;
using System;

namespace Examples.Bind
{
   class PetsInNeighborhood
   {
      record Pet(string Name)
      {
         public static implicit operator Pet(string s) => new (s);
      }

      record Neighbor(string Name, IEnumerable<Pet> Pets);

      internal static void _main_1()
      {
         var neighbours = new Neighbor[]
         {
            new (Name: "John", Pets: new Pet[] {"Fluffy", "Thor"}),
            new (Name: "Tim", Pets: Array.Empty<Pet>()),
            new (Name: "Carl", Pets: new Pet[] {"Sybil"}),
         };

         IEnumerable<IEnumerable<Pet>> nested = neighbours.Map(n => n.Pets);
         IEnumerable<Pet> flat = neighbours.Bind(n => n.Pets);
      }
   }
}