using System;
using System.Collections.Generic;

namespace GiteEF.Models;

public partial class HotelRoom
{
    public int RoomNumber { get; set; }

    public int MaxGuests { get; set; }

    public decimal Price { get; set; }

    public int? Floor { get; set; }

    public int? SquareMeters { get; set; }

    public int? NumberOfBeds { get; set; }

    public bool? IsDoubleBed { get; set; }

    public bool? HasAirConditioning { get; set; }

    public bool? HasHeating { get; set; }

    public bool? HasWifi { get; set; }

    public bool? HasTelevision { get; set; }

    public bool? IsWheelchairAccessible { get; set; }

    public bool? IsSmokingAllowed { get; set; }
}
