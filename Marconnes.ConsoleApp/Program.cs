namespace Marconnes.ConsoleApp
{
    public class Program
    {
        // AddRoomUser should be static and have a return type (void)
        public static void AddRoomUser()
        {
            HotelRoom room = new HotelRoom();

            Console.WriteLine("Wat is het kamernummer? (bijv. 101 of 2B)");
            room.RoomNumber = Console.ReadLine();

            Console.WriteLine("Hoeveel gasten mogen er maximaal in?");
            string inputGuests = Console.ReadLine();
            room.MaxGuests = int.Parse(inputGuests);

            Console.WriteLine("Wat is de prijs per nacht?");
            string inputPrice = Console.ReadLine();
            room.Price = decimal.Parse(inputPrice);

            Console.WriteLine($"Bezig met toevoegen: Kamer {room.RoomNumber}, {room.MaxGuests} pers, €{room.Price}");

            DAL dAL = new DAL();
            dAL.AddHotelRoom(room);

            Console.WriteLine("Kamer succesvol toegevoegd aan de database!");
            Console.ReadLine();
            Console.ReadLine();
        }
        public static void AddPlaceUser()
        {
            CampingPlace Place = new CampingPlace();

            Console.WriteLine("Wat is het Pleknummer? (bijv. 101 of 2B)");
            Place.PlaceNumber = Console.ReadLine();

            Console.WriteLine("Hoeveel gasten mogen er maximaal in?");
            string inputGuests = Console.ReadLine();
            Place.MaxGuests = int.Parse(inputGuests);

            Console.WriteLine("Wat is de prijs per nacht?");
            string inputPrice = Console.ReadLine();
            Place.Price = decimal.Parse(inputPrice);

            Console.WriteLine($"Bezig met toevoegen: plek {Place.PlaceNumber}, {Place.MaxGuests} pers, €{Place.Price}");

            DAL dAL = new DAL();
            dAL.AddCampingPlace(Place);

            Console.WriteLine("Plek succesvol toegevoegd aan de database!");
            Console.ReadLine();
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(@"Typ:
-'addRoom'  to add a new hotelroom.
-'addPlace' to add a new campingplace.
-'view' to view all rooms.");
            string command = Console.ReadLine();
            if (command == "addRoom")
            {
                AddRoomUser();
            }
            if (command == "addPlace")
            {
                AddPlaceUser();
            }
            else
            {
                Console.WriteLine("Unknown command.");
            }
        }
    }
}
