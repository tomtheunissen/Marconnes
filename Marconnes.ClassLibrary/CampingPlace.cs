using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ConsoleApp
{
    public class CampingPlace
    {
        public int PlaceID { get; set; }
        public string? PlaceNumber { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }


        public CampingPlace()
        {

        }


        public CampingPlace(string placeNumber, int maxGuests, decimal price)
        {
            PlaceNumber = placeNumber;
            MaxGuests = maxGuests;
            Price = price;
        }
    }
}

