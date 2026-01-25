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

        [HttpGet("Search")]
        public async Task<IActionResult> GetSearch([FromQuery] int zoekterm)
        {
            var hotelSearch = await _service.GetHotelData(zoekterm);
            var campingSearch = await _service.GetCampingData(zoekterm);

            // 3. Geef beide resultaten terug
            return Ok(new
            {
                Hotel = hotelSearch,
                Camping = campingSearch
            });
        }


        [HttpGet("combined")]
        public async Task<IActionResult> GetCombined()
        {
            var hotel = await _service.GetHotelData();
            var camping = await _service.GetCampingData();

            return Ok(new
            {
                Hotel = hotel,
                Camping = camping
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
    }
}
