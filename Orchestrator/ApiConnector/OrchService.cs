using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Orchestrator.ApiConnector
{
    public class OrchService
    {
        private readonly IHttpClientFactory _clientfactory;

        public OrchService(IHttpClientFactory clientfactory)
        {
            _clientfactory = clientfactory;
        }

        //Get all places
        //Hotel
        //Camping
        //Gite
        public async Task<string> GetHotelData()
        {
            var client = _clientfactory.CreateClient("HotelAPI");
            var response = await client.GetAsync("api/HotelRoom");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetCampingData()
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            var response = await client.GetAsync("api/Data/all_Camping");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetGiteData()
        {
            var client = _clientfactory.CreateClient("GiteAPI");
            var response = await client.GetAsync("gite/get/all");
            return await response.Content.ReadAsStringAsync();
        }

        //Search by placenumber
        //Hotel
        //Camping
        //Gite
        public async Task<string> GetHotelData(int? zoekterm = null)
        {
            var client = _clientfactory.CreateClient("HotelAPI");
            string url = "api/HotelRoom";

            if (zoekterm.HasValue)
            {
                url += $"/{zoekterm}";
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetCampingData(int? zoekterm = null)
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            string url = "api/Data/all_Camping";

            if (zoekterm.HasValue)
            {
                url = $"api/Data/zoek/{zoekterm}";
            }

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var foutmelding = await response.Content.ReadAsStringAsync();
                return $"FOUT ({response.StatusCode}): {foutmelding}";
            }

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetGiteData(int? zoekterm = null)
        {
            var client = _clientfactory.CreateClient("GiteAPI");
            string url = "gite/get/all";
            if (zoekterm.HasValue)
            {
                url = $"gite/get/{zoekterm}";
            }
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }


        //Reserveringen
        //get all
        //add
        //get all (neat)

        public async Task<string> GetAllReserveringen()
        {
            var client = _clientfactory.CreateClient("CampingAPI");

            string url = "api/data/all_reserveringen";

            var response = await client.GetAsync(url);

            // Check of het gelukt is
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> AddReservering(JsonObject json)
        {
            var campingClient = _clientfactory.CreateClient("CampingAPI");

            if (json["NieuweGebruiker"] != null)
            {
                Console.WriteLine("[ORCH] Nieuwe gebruiker gedetecteerd. Aanmaken...");

                var userNode = json["NieuweGebruiker"];

                var userResponse = await campingClient.PostAsJsonAsync("api/Data/add/gebruiker", userNode);

                if (!userResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("[FOUT] Kon gebruiker niet aanmaken.");
                    return false; // Stop het hele proces
                }

                var createdUser = await userResponse.Content.ReadFromJsonAsync<JsonObject>();
                int nieuwId = (int?)(createdUser["GebruikerId"] ?? createdUser["Gebruiker_id"] ?? createdUser["id"] ?? createdUser["placeNumber"]) ?? 0;
                Console.WriteLine($"[ORCH] Gebruiker aangemaakt met ID: {nieuwId}");
                json["GebruikerId"] = nieuwId;
            }

            if (json["Accomodatie"] == null) return false;
            int accommodatieNummer = (int)json["Accomodatie"];
            bool bestaat = false;

            // Hotel (100-199)
            if (accommodatieNummer >= 100 && accommodatieNummer <= 199)
            {
                var hotelClient = _clientfactory.CreateClient("HotelAPI");
                var check = await hotelClient.GetAsync($"api/HotelRoom/{accommodatieNummer}");
                if (check.IsSuccessStatusCode) bestaat = true;
            }
            // Camping (200+)
            else if (accommodatieNummer >= 200)
            {
                // Gebruik de nieuwe 'zoek' route met :int
                var check = await campingClient.GetAsync($"api/Data/zoek/{accommodatieNummer}");
                if (check.IsSuccessStatusCode) bestaat = true;
            }
            // Gite (0-99)
            else if (accommodatieNummer >= 0 && accommodatieNummer < 100)
            {
                var hotelClient = _clientfactory.CreateClient("GiteAPI");
                var check = await hotelClient.GetAsync($"gite/get/{accommodatieNummer}");
                if (check.IsSuccessStatusCode) bestaat = true;
            }

            if (!bestaat)
            {
                Console.WriteLine($"Fout: Accommodatie {accommodatieNummer} bestaat niet.");
                return false;
            }

            // STAP 4: Reservering opslaan
            // Let op: We sturen nu het json object door waar 'GebruikerId' zojuist is ingevuld
            var response = await campingClient.PostAsJsonAsync("api/Data/add/reservering", json);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<object>> GetVerrijkteReserveringen()
        {
            var campingClient = _clientfactory.CreateClient("CampingAPI");
            var hotelClient = _clientfactory.CreateClient("HotelAPI");
            var giteClient = _clientfactory.CreateClient("GiteAPI");

            var response = await campingClient.GetAsync("api/Data/all_reserveringen");
            if (!response.IsSuccessStatusCode) return null;
            var reserveringen = await response.Content.ReadFromJsonAsync<JsonArray>();
            var usersResponse = await campingClient.GetAsync("api/Data/all_gebruikers");

            JsonArray gebruikersLijst = null;
            if (usersResponse.IsSuccessStatusCode)
            {
                gebruikersLijst = await usersResponse.Content.ReadFromJsonAsync<JsonArray>();
            }

            var resultaatLijst = new List<object>();

            foreach (var res in reserveringen)
            {
                object details = null;
                string type = "Onbekend";

                // Waardes ophalen (veilig voor hoofdletters/kleine letters)
                int accommodatieNummer = (int?)(res["Accomodatie"] ?? res["accomodatie"]) ?? 0;
                int gebruikerId = (int?)(res["GebruikerId"] ?? res["Gebruiker_id"] ?? res["gebruiker_id"]) ?? 0;
                int reserveringId = (int?)(res["ReserveringId"] ?? res["Reservering_id"]) ?? 0;

                // --- LOGICA ---
                if (accommodatieNummer >= 100 && accommodatieNummer <= 199)
                {
                    type = "Hotel";
                    var hotelResp = await hotelClient.GetAsync($"api/HotelRoom/{accommodatieNummer}");
                    if (hotelResp.IsSuccessStatusCode)
                        details = await hotelResp.Content.ReadFromJsonAsync<object>();
                }
                else if (accommodatieNummer >= 200)
                {
                    type = "Camping";
                    // Check navigatie of haal los op
                    if (res["AccomodatieNavigation"] != null)
                    {
                        details = res["AccomodatieNavigation"];
                    }
                    else
                    {
                        // URL volgens Swagger -> 'zoek/{id}'
                        var campResp = await campingClient.GetAsync($"api/Data/zoek/{accommodatieNummer}");
                        if (campResp.IsSuccessStatusCode)
                        {
                            try
                            {
                                // Soms is het een lijst, soms een object. We proberen beide.
                                var lijst = await campResp.Content.ReadFromJsonAsync<JsonArray>();
                                if (lijst != null && lijst.Count > 0) details = lijst[0];
                            }
                            catch
                            {
                                details = await campResp.Content.ReadFromJsonAsync<object>();
                            }
                        }
                    }
                }
                else if (accommodatieNummer >= 0 && accommodatieNummer < 100)
                {
                    type = "Gite";
                    var giteResp = await giteClient.GetAsync($"gite/get/{accommodatieNummer}");
                    if (giteResp.IsSuccessStatusCode)
                        details = await giteResp.Content.ReadFromJsonAsync<object>();
                }

                // --- GEBRUIKER KOPPELEN ---
                object gekoppeldeGebruiker = null;
                if (gebruikersLijst != null && gebruikerId > 0)
                {
                    // Zoek op ID (houd rekening met 'Gebruiker_id' uit je DB screenshot)
                    var user = gebruikersLijst.FirstOrDefault(u =>
                        (int?)(u["GebruikerId"] ?? u["Gebruiker_id"] ?? u["gebruiker_id"] ?? u["Id"]) == gebruikerId
                    );
                    if (user != null) gekoppeldeGebruiker = user;
                }

                resultaatLijst.Add(new
                {
                    ReserveringId = reserveringId,
                    Type = type,
                    Begindatum = res["Begindatum"]?.ToString(),
                    Einddatum = res["Einddatum"]?.ToString(),
                    AccommodatieNummer = accommodatieNummer,
                    Details = details,
                    Gebruiker = gekoppeldeGebruiker
                });
            }

            return resultaatLijst;
        }


        //Search
        //Camping Plek
        //
        public async Task<string?> ZoekCampingPlek(string nummer)
        {
            var client = _clientfactory.CreateClient("CampingAPI");

            // WIJZIGING: Exact zoals in Swagger (zonder /camping ertussen)
            var response = await client.GetAsync($"api/Data/zoek/{nummer}");

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }

        // Gebruikers
        // Get all
        // Search by name
        public async Task<string> GetAllGebruikers()
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            var response = await client.GetAsync($"api/Data/all_gebruikers");

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ZoekGebruikerOpNaam(string naam)
        {
            var client = _clientfactory.CreateClient("CampingAPI");

            // We gebruiken hier dezelfde zoek-URL als bij de campingplekken.
            // Omdat 'naam' tekst is (en geen getal), pakt de Backend automatisch de gebruikers-zoekfunctie.
            var response = await client.GetAsync($"api/Data/zoek/{naam}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }


        //Updates
        //camping plek
        //hotel kamer
        //gite
        public async Task<bool> UpdateCampingPlek(int id, JsonObject campingData)
        {
            var client = _clientfactory.CreateClient("CampingAPI");

            campingData["PlaceNumber"] = id;

            var response = await client.PutAsJsonAsync($"api/Data/update/{id}", campingData);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateHotelKamer(int roomNumber, JsonObject hotelData)
        {
            var client = _clientfactory.CreateClient("HotelAPI");

            // We zorgen dat het ID in de JSON gelijk is aan de URL (verplicht voor de PUT)
            hotelData["RoomNumber"] = roomNumber;

            // URL voorbeeld: api/HotelRoom/105
            var response = await client.PutAsJsonAsync($"api/HotelRoom/{roomNumber}", hotelData);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateGiteKamer(int giteNumber, JsonObject gitedata)
        {
            var client = _clientfactory.CreateClient("GiteAPI");

            // We zorgen dat het ID in de JSON gelijk is aan de URL (verplicht voor de PUT)
            gitedata["giteNumber"] = giteNumber;

            // URL voorbeeld: api/HotelRoom/105
            var response = await client.PutAsJsonAsync($"gite/put/{giteNumber}", gitedata);

            return response.IsSuccessStatusCode;
        }


        // Delete reservering
        public async Task<bool> DeleteReservering(int id)
        {
            var client = _clientfactory.CreateClient("CampingAPI");

            // Check de route in DataController: [HttpDelete("delete_reserveringen/{ReserveringId}")]
            var response = await client.DeleteAsync($"api/Data/delete_reserveringen/{id}");

            return response.IsSuccessStatusCode;
        }

    }
}