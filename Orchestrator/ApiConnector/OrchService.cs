namespace Orchestrator.ApiConnector
{
    public class OrchService
    {
        private readonly IHttpClientFactory _clientfactory;

        public OrchService(IHttpClientFactory clientfactory)
        {
            _clientfactory = clientfactory;
        }

        public async Task<string> GetHotelData()
        { 
            var client = _clientfactory.CreateClient("HotelAPI");
            var response = await client.GetAsync("api/HotelRoom");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetCampingData()
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            var response = await client.GetAsync("api/Data");
            return await response.Content.ReadAsStringAsync();
        }

    }
}
