using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Orchestrator.ApiConnector
{
    public class OrchService
    {
        private readonly IHttpClientFactory _clientfactory;

        //belastingen
        private decimal _btwPercentageHotel = 9.0m; //percentage
        private decimal _toeristenBelastingHotel = 0.50m; //euro
        private decimal _toeristenBelastingCamping = 0.25m; //euro
        private decimal _btwPercentageGite = 0.0m; //euro

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
            var hotelClient = _clientfactory.CreateClient("HotelAPI");
            var campingClient = _clientfactory.CreateClient("CampingAPI");

            // hotel kamers ophalen
            var hotelResponse = await hotelClient.GetAsync("api/HotelRoom/all");
            if (!hotelResponse.IsSuccessStatusCode) return "[]";
            var kamers = await hotelResponse.Content.ReadFromJsonAsync<JsonArray>();

            // haal reserveringen op
            var reserveringResponse = await campingClient.GetAsync("api/Data/all_reserveringen");
            JsonArray? alleReserveringen = null;
            if (reserveringResponse.IsSuccessStatusCode)
            {
                alleReserveringen = await reserveringResponse.Content.ReadFromJsonAsync<JsonArray>();
            }

            // haal gebruikers op
            var gebruikersResponse = await campingClient.GetAsync("api/Data/all_gebruikers");
            JsonArray? alleGebruikers = null;
            if (gebruikersResponse.IsSuccessStatusCode)
            {
                alleGebruikers = await gebruikersResponse.Content.ReadFromJsonAsync<JsonArray>();
            }

            // combineer data
            if (kamers != null && alleReserveringen != null)
            {
                foreach (var kamer in kamers)
                {
                    int kamerId = (int?)(kamer["id"] ?? kamer["Id"] ?? kamer["RoomNumber"] ?? kamer["roomNumber"]) ?? 0;

                    if (kamerId > 0)
                    {
                        // Zoek reserveringen voor kamer
                        var boekingenVoorDezeKamer = alleReserveringen.Where(r =>
                        {
                            int resAcc = (int?)(r["Accomodatie"] ?? r["accomodatie"] ?? r["Accommodation"]) ?? 0;
                            return resAcc == kamerId;
                        }).Select(x => x.DeepClone()).Cast<JsonObject>().ToList();

                        // Koppel Gebruiker aan Reservering
                        if (alleGebruikers != null)
                        {
                            foreach (var boeking in boekingenVoorDezeKamer)
                            {
                                int gebruikerId = (int?)(boeking["GebruikerId"] ?? boeking["gebruiker_id"] ?? boeking["Gebruiker_id"]) ?? 0;

                                if (gebruikerId > 0)
                                {
                                    var gebruiker = alleGebruikers.FirstOrDefault(u =>
                                        (int?)(u["GebruikerId"] ?? u["id"] ?? u["Id"]) == gebruikerId
                                    );

                                    if (gebruiker != null)
                                    {
                                        boeking["Gebruiker"] = gebruiker.DeepClone();
                                    }
                                }
                            }
                        }

                        kamer["Reserveringen"] = new JsonArray(boekingenVoorDezeKamer.ToArray());
                    }
                }

                return kamers.ToJsonString();
            }

            // Fallback
            return await hotelResponse.Content.ReadAsStringAsync();
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
            var response = await client.GetAsync("api/Gites/all_gites");
            return await response.Content.ReadAsStringAsync();
        }

        //Search by placenumber
        //Hotel
        //Camping
        //Gite
        public async Task<string> GetHotelData(int? zoekterm = null)
        {
            var client = _clientfactory.CreateClient("HotelAPI");
            string url = "api/HotelRoom/all";

            if (zoekterm.HasValue)
            {
                url = $"api/HotelRoom/search/{zoekterm}";
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
            string url = "api/Gites/all_gites";
            if (zoekterm.HasValue)
            {
                url = $"api/Gites/{zoekterm}";
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
            Console.WriteLine("[ORCH] Start Reservering (Smart Check)");
            var campingClient = _clientfactory.CreateClient("CampingAPI");

            // haal gegevens invulveld op
            JsonNode? GetCaseInsensitive(JsonObject obj, string key)
            {
                return obj.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            }

            // Datums 
            DateOnly nieuwBegin, nieuwEind;
            string? beginStr = GetCaseInsensitive(json, "Begindatum")?.ToString();
            string? eindStr = GetCaseInsensitive(json, "Einddatum")?.ToString();

            if (string.IsNullOrEmpty(beginStr) || string.IsNullOrEmpty(eindStr)) return false;
            if (!DateOnly.TryParse(beginStr, out nieuwBegin) || !DateOnly.TryParse(eindStr, out nieuwEind)) return false;

            // check of nieuwe gebruiker ingevuld of gebruiker Id
            var nieuweGebruikerNode = GetCaseInsensitive(json, "NieuweGebruiker");
            int huidigId = (int?)(GetCaseInsensitive(json, "GebruikerId")) ?? 0;

            if (nieuweGebruikerNode != null)
            {
                // Voeg nieuwe gebruiker toe
                var userResponse = await campingClient.PostAsJsonAsync("api/Data/add/gebruiker", nieuweGebruikerNode);
                if (userResponse.IsSuccessStatusCode)
                {
                    var createdUser = await userResponse.Content.ReadFromJsonAsync<JsonObject>();
                    huidigId = (int?)(GetCaseInsensitive(createdUser, "GebruikerId") ?? GetCaseInsensitive(createdUser, "id")) ?? 0;
                }
                else
                {
                    // zoek gebruiker
                    string naam = nieuweGebruikerNode["naam"]?.ToString() ?? "";
                    var zoek = await campingClient.GetAsync($"api/Data/zoek/{Uri.EscapeDataString(naam)}");
                    if (zoek.IsSuccessStatusCode)
                    {
                        var users = await zoek.Content.ReadFromJsonAsync<List<JsonObject>>();
                        var u = users?.LastOrDefault();
                        if (u != null) huidigId = (int?)(GetCaseInsensitive(u, "GebruikerId") ?? GetCaseInsensitive(u, "id")) ?? 0;
                    }
                }
            }

            if (huidigId == 0) { Console.WriteLine("[FOUT] Geen GebruikerId."); return false; }

            // BESCHIKBAARHEIDS CHECK
            int accommodatieNummer = (int?)(GetCaseInsensitive(json, "Accomodatie")) ?? 0;
            if (accommodatieNummer == 0) return false;

            bool plekBestaat = false;

            // HOTEL (100-199)
            if (accommodatieNummer >= 100 && accommodatieNummer <= 199)
            {
                Console.WriteLine($"[ORCH] Checken bij Hotel API voor kamer {accommodatieNummer}...");
                var hotelClient = _clientfactory.CreateClient("HotelAPI");

                // Check kamer bestaat
                var resp = await hotelClient.GetAsync($"api/HotelRoom/search/{accommodatieNummer}");

                if (resp.IsSuccessStatusCode)
                {
                    plekBestaat = true;
                    Console.WriteLine("[ORCH] Hotelkamer gevonden! Nu checken op dubbele boekingen...");

                    // B. Check dubbele boekingen in database
                    var resResp = await campingClient.GetAsync("api/Data/all_reserveringen");
                    if (resResp.IsSuccessStatusCode)
                    {
                        var alleReserveringen = await resResp.Content.ReadFromJsonAsync<JsonArray>();

                        // Filter: Alleen reserveringen voor DEZE kamer
                        var boekingenVoorDezeKamer = alleReserveringen?.Where(r =>
                            (int?)(r["Accomodatie"] ?? r["accomodatie"]) == accommodatieNummer
                        );

                        if (boekingenVoorDezeKamer != null)
                        {
                            foreach (var reservering in boekingenVoorDezeKamer)
                            {
                                string? sStr = reservering["begindatum"]?.ToString();
                                string? eStr = reservering["einddatum"]?.ToString();

                                if (sStr != null && eStr != null)
                                {
                                    DateOnly oudStart = DateOnly.Parse(sStr);
                                    DateOnly oudEind = DateOnly.Parse(eStr);

                                    // OVERLAP CHECK
                                    if (nieuwBegin < oudEind && nieuwEind > oudStart)
                                    {
                                        Console.WriteLine($"[FOUT] Dubbele boeking op hotelkamer {accommodatieNummer}.");
                                        throw new InvalidOperationException($"De kamer is al geboekt van {oudStart} tot {oudEind}.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // CAMPING (200-299)
            else if (accommodatieNummer >= 200 && accommodatieNummer <= 300)
            {
                Console.WriteLine($"[ORCH] Checken bij Camping API voor plek {accommodatieNummer}...");
                var resp = await campingClient.GetAsync($"api/Data/zoek/{accommodatieNummer}");

                if (resp.IsSuccessStatusCode)
                {
                    plekBestaat = true;

                    var plekkenLijst = await resp.Content.ReadFromJsonAsync<List<JsonObject>>();
                    var bestaandeLijst = plekkenLijst?.FirstOrDefault()?["reserveringens"]?.AsArray();

                    if (bestaandeLijst != null)
                    {
                        foreach (var reservering in bestaandeLijst)
                        {
                            string? sStr = reservering["begindatum"]?.ToString();
                            string? eStr = reservering["einddatum"]?.ToString();

                            if (sStr != null && eStr != null)
                            {
                                DateOnly oudStart = DateOnly.Parse(sStr);
                                DateOnly oudEind = DateOnly.Parse(eStr);

                                if (nieuwBegin < oudEind && nieuwEind > oudStart)
                                {
                                    Console.WriteLine($"[FOUT] Dubbele boeking op campingplek {accommodatieNummer}.");
                                    throw new InvalidOperationException($"Dubbele boeking: Plek {accommodatieNummer} is al bezet van {oudStart} tot {oudEind}.");
                                }
                            }
                        }
                    }
                }
            }
            // GITE (< 100)
            else
            {
                var giteClient = _clientfactory.CreateClient("GiteAPI");
                var resp = await giteClient.GetAsync($"api/Gites/{accommodatieNummer}");
                if (resp.IsSuccessStatusCode) plekBestaat = true;
            }

            // foutmelding als plek niet bestaat
            if (!plekBestaat)
            {
                Console.WriteLine($"[FOUT] Accommodatie {accommodatieNummer} niet gevonden bij de bron.");
                return false;
            }

            // PRIJS GEGEVENS OPHALEN
            int volw = (int?)(GetCaseInsensitive(json, "Volwassenen")) ?? 0;
            int k07 = (int?)(GetCaseInsensitive(json, "Kinderen07")) ?? 0;
            int k712 = (int?)(GetCaseInsensitive(json, "Kinderen712")) ?? 0;

            // Prijs berekenen
            decimal prijs = BerekenTotaalPrijs(accommodatieNummer, nieuwBegin, nieuwEind, volw, k07, k712);
            Console.WriteLine($"[INFO] Prijs: {prijs}");

            var payload = new
            {
                // Vul de velden in zoals verwacht door de API
                Accomodatie = accommodatieNummer,
                GebruikerId = huidigId,
                Begindatum = beginStr,
                Einddatum = eindStr,
                Volwassenen = volw,
                Kinderen07 = k07,
                Kinderen712 = k712,
                TotaalPrijs = prijs
            };

            // Opslaan in database
            var postResp = await campingClient.PostAsJsonAsync("api/Data/add/reservering", payload);

            if (!postResp.IsSuccessStatusCode)
            {
                string err = await postResp.Content.ReadAsStringAsync();
                Console.WriteLine($"[FOUT] Opslaan mislukt ({postResp.StatusCode}): {err}");
                return false;
            }

            Console.WriteLine("[SUCCES] Reservering Opgeslagen!");
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

                int accommodatieNummer = (int?)(res["Accomodatie"] ?? res["accomodatie"]) ?? 0;
                int gebruikerId = (int?)(res["GebruikerId"] ?? res["Gebruiker_id"] ?? res["gebruiker_id"]) ?? 0;
                int reserveringId = (int?)(res["ReserveringId"] ?? res["Reservering_id"]) ?? 0;

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

                // GEBRUIKER KOPPELEN
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

            var response = await client.PutAsJsonAsync($"api/HotelRoom/update/{roomNumber}", hotelData);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateGiteKamer(int giteNumber, JsonObject gitedata)
        {
            var client = _clientfactory.CreateClient("GiteAPI");
            gitedata["GiteNumber"] = giteNumber;
            var response = await client.PutAsJsonAsync($"api/Gites/{giteNumber}", gitedata);
            return response.IsSuccessStatusCode;
        }

        // Delete reservering
        public async Task<bool> DeleteReservering(int id)
        {
            var client = _clientfactory.CreateClient("CampingAPI");
            var response = await client.DeleteAsync($"api/Data/delete_reserveringen/{id}");
            return response.IsSuccessStatusCode;
        }

        // Prijs berekenen
        private decimal BerekenTotaalPrijs(int Accomodatie, DateOnly begindatum, DateOnly einddatum, int volwassenen, int kinderen07, int kinderen712)
        {
            // Bereken aantal nachten
            int nachten = (einddatum.DayNumber - begindatum.DayNumber);
            if (nachten < 1) nachten = 1;

            decimal totaal = 0;

            // LOGICA CAMPING (nummers 200 en hoger)
            if (Accomodatie >= 200)
            {
                decimal prijsPerNacht = 7.50m; // Plaats
                prijsPerNacht += (volwassenen * 6.00m);
                prijsPerNacht += (kinderen07 * 4.00m);
                prijsPerNacht += (kinderen712 * 5.00m);
                prijsPerNacht += 7.50m; // Stroom

                // Toeristenbelasting Camping
                int aantalPersonen = volwassenen + kinderen07 + kinderen712;
                prijsPerNacht += (aantalPersonen * _toeristenBelastingCamping);

                totaal = prijsPerNacht * nachten;
            }
            // LOGICA HOTEL (Accommodatie nummers lager dan 200)
            else
            {
                int totaalPersonen = volwassenen + kinderen07 + kinderen712;
                decimal kamerPrijs = 55.00m; // Fallback prijs

                switch (totaalPersonen)
                {
                    case 1: kamerPrijs = 42.50m; break;
                    case 2: kamerPrijs = 55.00m; break;
                    case 3: kamerPrijs = 70.00m; break;
                    case 4: kamerPrijs = 88.00m; break;
                    case 5: kamerPrijs = 105.50m; break;
                }

                decimal basisBedrag = kamerPrijs * nachten;

                // BTW BEREKENING
                decimal btwFactor = 1 + (_btwPercentageHotel / 100m);
                decimal bedragMetBtw = basisBedrag * btwFactor;

                // Toeristenbelasting Hotel
                decimal toeristenBelasting = (totaalPersonen * _toeristenBelastingHotel) * nachten;
                totaal = bedragMetBtw + toeristenBelasting;
            }

            return Math.Round(totaal, 2);
        }
    }
}