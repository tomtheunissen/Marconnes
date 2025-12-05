namespace Marconnes.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 1. Maak een Room object aan in plaats van Band
            HotelRoom room = new HotelRoom();

            // 2. Vraag om het kamernummer (Dit is een string/tekst)
            Console.WriteLine("Wat is het kamernummer? (bijv. 101 of 2B)");
            room.RoomNumber = Console.ReadLine();

            // 3. Vraag om max aantal gasten (Dit is een int/getal)
            Console.WriteLine("Hoeveel gasten mogen er maximaal in?");
            string inputGuests = Console.ReadLine();
            room.MaxGuests = int.Parse(inputGuests);

            // 4. Vraag om de prijs (Dit is een decimal)
            Console.WriteLine("Wat is de prijs per nacht?");
            string inputPrice = Console.ReadLine();
            room.Price = decimal.Parse(inputPrice);

            // Even laten zien wat we hebben ingevuld
            Console.WriteLine($"Bezig met toevoegen: Kamer {room.RoomNumber}, {room.MaxGuests} pers, €{room.Price}");

            // 5. Stuur naar de database via de DAL
            DAL dAL = new DAL();
            dAL.AddHotelRoom(room); // Let op: AddRoom, niet AddBand!

            Console.WriteLine("Kamer succesvol toegevoegd aan de database!");
            Console.ReadLine(); // Zorgt dat het scherm niet meteen sluit
        }
    }
}