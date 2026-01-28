using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CampingEF.Models;

public partial class Gebruiker
{
    public int GebruikerId { get; set; }

    public string Naam { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefoonnummer { get; set; }

    [JsonIgnore]
    public virtual ICollection<Reserveringen> Reserveringens { get; set; } = new List<Reserveringen>();
}
