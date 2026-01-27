using System;
using System.Collections.Generic;

namespace CampingEF.Models;

public partial class Reserveringen
{
    public int ReserveringId { get; set; }
    public int GebruikerId { get; set; }
    public int Accomodatie { get; set; }
    public DateOnly Begindatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public int Volwassenen { get; set; }
    public int? Kinderen07 { get; set; }
    public int? Kinderen712 { get; set; }

    public virtual CampingPlace? AccomodatieNavigation { get; set; }
    public virtual Gebruiker? Gebruiker { get; set; }
}
