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