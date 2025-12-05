using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ConsoleApp
{
    public class Band
    {
        // PROPERTIES
        // Dit zijn private attributen met een getmethode en een setmethode.
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public int EstablishedYear { get; set; }

        // CONSTRUCTORS
        // Een constructor is een speciale methode die wordt aangeroepen wanneer een object van een klasse wordt gemaakt.
        // Het heeft dezelfde naam als de klasse en heeft geen returntype.
        // Base constructor (geen parameters, is er ook als hij er niet letterlijk staat.)
        public Band()
        {

        }

        // Constructor overloading (met 2 parameters) voor het aanmaken van een band zonder id
        // (id wordt automatisch gegenereerd in de database)
        public Band(string name, string genre)
        {
            Name = name;
            Genre = genre;
        }

        // Constructor overloading (met 3 parameters)
        public Band(int id, string name, string genre)
        {
            Id = id;
            Name = name;
            Genre = genre;
        }

        // Constructor overloading (met 4 parameters)
        public Band(int id, string name, string genre, int establishedYear)
        {
            Id = id;
            Name = name;
            Genre = genre;
            EstablishedYear = establishedYear;
        }

        // TOSTRING METHOD (Is wat je ziet als je een cw van de class doet.)
        public override string? ToString()
        {
            return $"Id: {Id}, Name: {Name}, Genre: {Genre}, Established Year: {EstablishedYear}";
        }

    }
}
