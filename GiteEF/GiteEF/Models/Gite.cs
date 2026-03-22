using System;
using System.Collections.Generic;

namespace GiteEF.Models;

public partial class Gite
{
    public int GiteNumber { get; set; }

    public int MaxGuests { get; set; }

    public decimal Price { get; set; }

    public int? NumberOfBedrooms { get; set; }

    public int? NumberOfBathrooms { get; set; }

    public bool? HasLivingRoom { get; set; }

    public bool? HasKitchen { get; set; }

    public bool? HasTerrace { get; set; }

    public bool? HasPoolAcces { get; set; }

    public bool? HasSaunaAcces { get; set; }

    public bool? BreakfastAvailable { get; set; }

    public bool? HasWifi { get; set; }

    public bool? ArePetsAllowed { get; set; }
}
