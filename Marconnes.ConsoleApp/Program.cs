namespace Marconnes.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
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
    }
}