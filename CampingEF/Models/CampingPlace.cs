using System;
using System.Collections.Generic;

namespace CampingEF.Models;

public class CampingPlace
{
    public int PlaceId { get; set; }

    public string PlaceNumber { get; set; } = null!;

    public int MaxGuests { get; set; }

    public decimal Price { get; set; }

    public bool? HasElectricity { get; set; }

    public int? Ampere { get; set; }

    public bool? HasWaterConnection { get; set; }

    public bool? HasSewageDrain { get; set; }

    public int? SurfaceArea { get; set; }

    public string? GroundType { get; set; }

    public bool? IsShaded { get; set; }

    public bool? IsCarAllowed { get; set; }

    public bool? ArePetsAllowed { get; set; }
}
