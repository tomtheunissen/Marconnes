using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ConsoleApp
{
    public class HotelRoom
    {
        // De namen moeten PRECIES matchen met je Database kolommen
        public int RoomID { get; set; }
        public string? RoomNumber { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }

        // Lege constructor (nodig voor database en API)
        public HotelRoom()
        {

        }

        // Handige constructor om snel een kamer te maken (zonder ID)
        public HotelRoom(string roomNumber, int maxGuests, decimal price)
        {
            RoomNumber = roomNumber;
            MaxGuests = maxGuests;
            Price = price;
        }
    }
}