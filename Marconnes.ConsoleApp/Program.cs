namespace Marconnes.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Band band = new Band();
            Console.WriteLine("Wat is de bandnaam?");
            band.Name = Console.ReadLine();
            Console.WriteLine("Wat is het genre?");
            band.Genre = Console.ReadLine();
            Console.WriteLine("Wat is het jaar waarin de band is opgericht?");
            band.EstablishedYear = Int32.Parse(Console.ReadLine());
            Console.WriteLine(band.ToString());

            DAL dAL = new DAL();
            dAL.AddBand(band);

        }
    }
}