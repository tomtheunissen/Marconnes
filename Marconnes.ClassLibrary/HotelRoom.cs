using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ConsoleApp
{
    public class HotelRoom
    {
        public int RoomID { get; set; }
        public string? RoomNumber { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }
        public int Floor { get; set; }
        public int SquareMeters { get; set; }
        public int NumberOfBeds { get; set; }
        public bool IsDoubleBed { get; set; }

        public bool HasAirConditioning { get; set; }
        public bool HasHeating { get; set; }

        public bool HasWifi { get; set; }
        public bool HasTelevision { get; set; }

        public bool IsWheelchairAccessible { get; set; }
        public bool IsSmokingAllowed { get; set; }



        public HotelRoom()
        {
        }

        public HotelRoom(string roomNumber, int maxGuests, decimal price)
        {
            RoomNumber = roomNumber;
            MaxGuests = maxGuests;
            Price = price;
        }
    }
}