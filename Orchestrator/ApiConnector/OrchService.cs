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
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> AddReservering(JsonObject json)
        {
            Console.WriteLine("\n[ORCH] --- Start Slimme Reservering (Met Herstel) ---");
            var campingClient = _clientfactory.CreateClient("CampingAPI");

            // Helper functie
            JsonNode? GetCaseInsensitive(JsonObject obj, string key)
            {
                return obj.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            }

            // --- STAP 1: Datums veiligstellen ---
            string? beginStr = GetCaseInsensitive(json, "Begindatum")?.ToString();
            string? eindStr = GetCaseInsensitive(json, "Einddatum")?.ToString();

            // --- STAP 2: Gebruiker Aanmaken (Met Reddingsplan) ---
            var nieuweGebruikerNode = GetCaseInsensitive(json, "NieuweGebruiker");
            int huidigId = (int?)(GetCaseInsensitive(json, "GebruikerId")) ?? 0;

            if (nieuweGebruikerNode != null)
            {
                string naam = nieuweGebruikerNode["naam"]?.ToString() ?? "Onbekend";
                Console.WriteLine($"[ORCH] Gebruiker '{naam}' proberen aan te maken...");

                var userResponse = await campingClient.PostAsJsonAsync("api/Data/add/gebruiker", nieuweGebruikerNode);

                if (userResponse.IsSuccessStatusCode)
                {
                    // Scenario A: Het ging in één keer goed (onwaarschijnlijk met jouw backend)
                    var createdUser = await userResponse.Content.ReadFromJsonAsync<JsonObject>();
                    huidigId = (int?)(GetCaseInsensitive(createdUser, "GebruikerId") ?? GetCaseInsensitive(createdUser, "id")) ?? 0;
                }
                else
                {
                    // Scenario B: De backend crashte (500), maar heeft hem waarschijnlijk wel opgeslagen.
                    Console.WriteLine($"[INFO] Backend gaf foutmelding ({userResponse.StatusCode}). We starten Reddingsoperatie...");

                    // We zoeken de gebruiker op naam in de database
                    // Let op: Uri.EscapeDataString zorgt dat spaties (zoals in "Test Persoon") geen fouten geven in de URL
                    string veiligeNaam = Uri.EscapeDataString(naam);
                    var zoekResponse = await campingClient.GetAsync($"api/Data/zoek/{veiligeNaam}");

                    if (zoekResponse.IsSuccessStatusCode)
                    {
                        var gevondenUsers = await zoekResponse.Content.ReadFromJsonAsync<List<JsonObject>>();
                        // We pakken de laatste (meest recente) die matcht
                        var user = gevondenUsers?.LastOrDefault();

                        if (user != null)
                        {
                            huidigId = (int?)(GetCaseInsensitive(user, "GebruikerId") ?? GetCaseInsensitive(user, "id")) ?? 0;
                            Console.WriteLine($"[SUCCES] Herstel gelukt! Gebruiker gevonden met ID: {huidigId}");
                        }
                    }

                    if (huidigId == 0)
                    {
                        // Als we hem écht niet kunnen vinden, dan pas geven we het op.
                        throw new Exception($"CRITISCH: Gebruiker aanmaken mislukt én gebruiker niet gevonden in DB.");
                    }
                }
            }

            // --- STAP 3: De Reservering Maken ---
            if (huidigId == 0) throw new Exception("Geen GebruikerId beschikbaar.");

            var payloadObject = new
            {
                Accomodatie = (int?)(GetCaseInsensitive(json, "Accomodatie")) ?? 0,
                GebruikerId = huidigId,
                Begindatum = beginStr,
                Einddatum = eindStr,
                Volwassenen = (int?)(GetCaseInsensitive(json, "Volwassenen")) ?? 0,
                Kinderen07 = (int?)(GetCaseInsensitive(json, "Kinderen07")) ?? 0,
                Kinderen712 = (int?)(GetCaseInsensitive(json, "Kinderen712")) ?? 0
            };

            var response = await campingClient.PostAsJsonAsync("api/Data/add/reservering", payloadObject);

            if (!response.IsSuccessStatusCode)
            {
                string detail = await response.Content.ReadAsStringAsync();
                throw new Exception($"RESERVERING FOUT: {detail}");
            }

            return true;
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
                    if (res["AccomodatieNavigation"] != null)
                    {
                        details = res["AccomodatieNavigation"];
                    }
                    else
                    {
                        var campResp = await campingClient.GetAsync($"api/Data/zoek/{accommodatieNummer}");
                        if (campResp.IsSuccessStatusCode)
                        {
                            try
                            {
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
            hotelData["RoomNumber"] = roomNumber;
            var response = await client.PutAsJsonAsync($"api/HotelRoom/{roomNumber}", hotelData);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateGiteKamer(int giteNumber, JsonObject gitedata)
        {
            var client = _clientfactory.CreateClient("GiteAPI");
            gitedata["giteNumber"] = giteNumber;
            var response = await client.PutAsJsonAsync($"gite/put/{giteNumber}", gitedata);
            return response.IsSuccessStatusCode;
        }


        // Delete reservering
        public async Task<bool> DeleteReservering(int id)
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            var response = await client.DeleteAsync($"api/Data/delete_reserveringen/{id}");
            return response.IsSuccessStatusCode;
        }

    }
}