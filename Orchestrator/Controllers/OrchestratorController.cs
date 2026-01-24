using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.ApiConnector;
using System.Reflection.Metadata.Ecma335;

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
    }
}
