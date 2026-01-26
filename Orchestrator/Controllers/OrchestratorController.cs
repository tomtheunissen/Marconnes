using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.ApiConnector;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrchestratorController : ControllerBase
    {
        private readonly OrchService _service;

        public OrchestratorController(OrchService service)
        {
            _service = service;
        }

        //Endpoint
        [HttpGet("hotel")]
        public async Task<IActionResult> GetHotel()
        {
            var data = await _service.GetHotelData();
            return Ok(data);
        }

        [HttpGet("camping")]
        public async Task<IActionResult> GetCamping()
        {
            var data = await _service.GetCampingData();
            return Ok(data);
        }

        [HttpGet("Gite")]
        public async Task<IActionResult> GetGite()
        {
            var data = await _service.GetGiteData();
            return Ok(data);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> GetSearch([FromQuery] int zoekterm)
        {
            var hotelSearch = await _service.GetHotelData(zoekterm);
            var campingSearch = await _service.GetCampingData(zoekterm);
            var giteSearch = await _service.GetGiteData(zoekterm);

            // 3. Geef alle resultaten terug
            return Ok(new
            {
                Hotel = hotelSearch,
                Camping = campingSearch,
                Gite = giteSearch
            });
        }




        [HttpGet("combined")]
        public async Task<IActionResult> GetCombined()
        {
            var hotel = await _service.GetHotelData();
            var camping = await _service.GetCampingData();
            var gite = await _service.GetGiteData();

            return Ok(new
            {
                Hotel = hotel,
                Camping = camping,
                gite = gite
            });
        }

        [HttpGet("reserveringen")]
        public async Task<IActionResult> GetReserveringen()
        {
            var data = await _service.GetAllReserveringen();

            if (data == null)
            {
                return NotFound("Kon geen reserveringen ophalen.");
            }

            return Ok(data);
        }

        [HttpGet("reserveringen/details")]
        public async Task<IActionResult> GetDetails()
        {
            var data = await _service.GetVerrijkteReserveringen();
            if (data == null) return NotFound("Geen data gevonden.");
            return Ok(data);
        }


        [HttpPost("reserveringen")]
        public async Task<IActionResult> AddReservering([FromBody] ReserveringInput input)
        {
            var json = JsonSerializer.SerializeToNode(input).AsObject();

            bool gelukt = await _service.AddReservering(json);

            if (gelukt) return Ok("Reservering succesvol aangemaakt!");

            return BadRequest("Kon reservering niet maken.");
        }

        [HttpGet("gebruikers")]
        public async Task<IActionResult> GetGebruikers()
        {
            var data = await _service.GetAllGebruikers();
            if (data == null) return NotFound("Kon geen gebruikers ophalen.");
            return Ok(data);
        }

        [HttpGet("gebruikers/zoek/{naam}")]
        public async Task<IActionResult> ZoekGebruiker(string naam)
        {
            var data = await _service.ZoekGebruikerOpNaam(naam);

            if (string.IsNullOrEmpty(data))
            {
                return NotFound($"Geen gebruiker gevonden met de naam: {naam}");
            }

            return Ok(data);
        }

        [HttpPut("camping/{id}")]
        public async Task<IActionResult> UpdateCampingPlek(int id, [FromBody] CampingPlekInput input)
        {
            // 1. Zet input om naar JsonObject
            var json = JsonSerializer.SerializeToNode(input).AsObject();

            // 2. Stuur naar de service
            bool gelukt = await _service.UpdateCampingPlek(id, json);

            if (gelukt) return Ok($"Campingplek {id} succesvol gewijzigd.");

            // Foutafhandeling
            return BadRequest("Kon campingplek niet wijzigen. Check of het nummer bestaat.");
        }

        [HttpDelete("reserveringen/{id}")]
        public async Task<IActionResult> DeleteReservering(int id)
        {
            bool gelukt = await _service.DeleteReservering(id);

            if (gelukt) return Ok($"Reservering {id} is verwijderd.");

            // Als het niet lukt (bijv. id bestaat niet of server error)
            return NotFound("Kon reservering niet verwijderen. Check of het ID bestaat.");
        }

        [HttpPut("hotel/{roomNumber}")]
        public async Task<IActionResult> UpdateHotelKamer(int roomNumber, [FromBody] HotelKamerInput input)
        {
            var json = JsonSerializer.SerializeToNode(input).AsObject();

            bool gelukt = await _service.UpdateHotelKamer(roomNumber, json);

            if (gelukt) return Ok($"Hotelkamer {roomNumber} is gewijzigd.");

            return BadRequest("Kon hotelkamer niet wijzigen. Check of het nummer bestaat en de data klopt.");
        }

        [HttpPut("Gite/{giteNumber}")]
        public async Task<IActionResult> UpdateGiteKamer(int giteNumber, [FromBody] GiteKamerInput input)
        {
            var json = JsonSerializer.SerializeToNode(input).AsObject();

            bool gelukt = await _service.UpdateHotelKamer(giteNumber, json);

            if (gelukt) return Ok($"Gitekamer {giteNumber} is gewijzigd.");

            return BadRequest("Kon Gitekamer niet wijzigen. Check of het nummer bestaat en de data klopt.");
        }


        public class ReserveringInput
        {
            public int Accomodatie { get; set; }
            public DateOnly Begindatum { get; set; }
            public DateOnly Einddatum { get; set; }
            public int Volwassenen { get; set; }
            public int Kinderen { get; set; }

            // NIEUW: Dit mag leeg blijven (null)
            public int? GebruikerId { get; set; }
            public NieuweGebruikerInput? NieuweGebruiker { get; set; }
        }

        public class NieuweGebruikerInput
        {
            public string Naam { get; set; }
            public string Email { get; set; }
            public int Telefoonnummer { get; set; }
            public string? Adres { get; set; }
        }

        public class CampingPlekInput
        {

            public int MaxGuests { get; set; }
            public decimal Price { get; set; }
            public int SurfaceArea { get; set; } // Oppervlakte
            public bool HasElectricity { get; set; }
            public int Ampere { get; set; }
            public bool HasWaterConnection { get; set; }
            public bool HasSewageDrain { get; set; } // Riool
            public bool IsShaded { get; set; }
            public bool IsCarAllowed { get; set; }
            public bool ArePetsAllowed { get; set; }
            public string GroundType { get; set; } // Gras, Zand, etc.
        }

        public class HotelKamerInput
        {
            // Verplichte velden (Allow Nulls = Uit)
            public int MaxGuests { get; set; }
            public decimal Price { get; set; }

            // Optionele velden (Allow Nulls = Aan, dus we gebruiken een vraagteken)
            public int? Floor { get; set; }
            public int? SquareMeters { get; set; }
            public int? NumberOfBeds { get; set; }

            // Bit in SQL wordt bool in C#
            public bool? IsDoubleBed { get; set; }
            public bool? HasAirConditioning { get; set; }
            public bool? HasHeating { get; set; }
            public bool? HasWifi { get; set; }
            public bool? HasTelevision { get; set; }
            public bool? IsWheelchairAccessible { get; set; }
            public bool? IsSmokingAllowed { get; set; }
        }

        public class GiteKamerInput
        {
            public int GiteNumber { get; set; }
            public decimal GitePrice { get; set; }
            public bool IsAvailable { get; set; }
            public string GiteAddress { get; set; }
            public int CapacityMin { get; set; }
            public int CapacityMax { get; set; }

            // Dit is het geneste object voor voorzieningen
            public GiteAmenities Amenities { get; set; }

            // Dit is de lijst (array) met bedden
            public List<GiteBed> Beds { get; set; }
        }

        public class GiteAmenities
        {
            public bool Wifi { get; set; }
            public bool Bath { get; set; }
            public bool Shower { get; set; }
            public bool HairDryer { get; set; }
            public bool SmallChild { get; set; }
            public bool Toiletries { get; set; }
            public bool Desk { get; set; }
            public bool Chair { get; set; }
            public bool Balcony { get; set; }
            public bool Sofa { get; set; }
            public bool SofaBed { get; set; }
            public bool MiniFridge { get; set; }
            public bool Kettle { get; set; }
            public bool Cuttlery { get; set; }
            public bool EatingArea { get; set; }
            public bool RoomService { get; set; }
        }

        public class GiteBed
        {
            public int Amount1PrBed { get; set; }
            public int Amount2PrBed { get; set; }
            public int Amount3PrBed { get; set; }
            public string BedSort { get; set; }
        }
    }
}
