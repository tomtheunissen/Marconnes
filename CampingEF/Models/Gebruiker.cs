using System;
using System.Collections.Generic;

namespace CampingEF.Models;

public partial class Gebruiker
{
    public int GebruikerId { get; set; }

    public string Naam { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefoonnummer { get; set; }

    public virtual ICollection<Reserveringen> Reserveringens { get; set; } = new List<Reserveringen>();
}
