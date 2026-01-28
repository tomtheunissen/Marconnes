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
            Console.WriteLine("[ORCH] Start Reservering (Silent Mode)");
            var campingClient = _clientfactory.CreateClient("CampingAPI");

            JsonNode? GetCaseInsensitive(JsonObject obj, string key)
            {
                return obj.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            }

            // STAP 1: Datum
            DateOnly nieuwBegin, nieuwEind;
            string? beginStr = GetCaseInsensitive(json, "Begindatum")?.ToString();
            string? eindStr = GetCaseInsensitive(json, "Einddatum")?.ToString();

            if (string.IsNullOrEmpty(beginStr) || string.IsNullOrEmpty(eindStr))
            {
                Console.WriteLine("[FOUT] Datums ontbreken.");
                return false;
            }

            try
            {
                nieuwBegin = DateOnly.Parse(beginStr);
                nieuwEind = DateOnly.Parse(eindStr);
            }
            catch
            {
                Console.WriteLine("[FOUT] Datums ongeldig formaat.");
                return false;
            }

            // STAP 2: Gebruiker Aanmaken/Zoeken
            var nieuweGebruikerNode = GetCaseInsensitive(json, "NieuweGebruiker");
            int huidigId = (int?)(GetCaseInsensitive(json, "GebruikerId")) ?? 0;

            if (nieuweGebruikerNode != null)
            {
                string naam = nieuweGebruikerNode["naam"]?.ToString() ?? "";
                var userResponse = await campingClient.PostAsJsonAsync("api/Data/add/gebruiker", nieuweGebruikerNode);

                if (userResponse.IsSuccessStatusCode)
                {
                    var createdUser = await userResponse.Content.ReadFromJsonAsync<JsonObject>();
                    huidigId = (int?)(GetCaseInsensitive(createdUser, "GebruikerId") ?? GetCaseInsensitive(createdUser, "id")) ?? 0;
                }
                else
                {
                    Console.WriteLine($"[INFO] Aanmaken faalde ({userResponse.StatusCode}). Start herstel via zoekopdracht...");

                    string veiligeNaam = Uri.EscapeDataString(naam);
                    var zoekResponse = await campingClient.GetAsync($"api/Data/zoek/{veiligeNaam}");

                    if (zoekResponse.IsSuccessStatusCode)
                    {
                        var gevondenUsers = await zoekResponse.Content.ReadFromJsonAsync<List<JsonObject>>();
                        var user = gevondenUsers?.LastOrDefault();
                        if (user != null)
                        {
                            huidigId = (int?)(GetCaseInsensitive(user, "GebruikerId") ?? GetCaseInsensitive(user, "id")) ?? 0;
                            Console.WriteLine($"[SUCCES] Herstel gelukt. Gebruiker ID: {huidigId}");
                        }
                    }

                    if (huidigId == 0)
                    {
                        Console.WriteLine("[FOUT] Gebruiker kon niet worden aangemaakt en niet worden gevonden.");
                        return false;
                    }
                }
            }

            // STAP 3: Dubbele Boeking Check
            int accommodatieNummer = (int?)(GetCaseInsensitive(json, "Accomodatie")) ?? 0;
            if (accommodatieNummer == 0)
            {
                Console.WriteLine("[FOUT] Geen accommodatienummer.");
                return false;
            }

            var plekCheck = await campingClient.GetAsync($"api/Data/zoek/{accommodatieNummer}");

            if (plekCheck.IsSuccessStatusCode)
            {
                var plekkenLijst = await plekCheck.Content.ReadFromJsonAsync<List<JsonObject>>();
                var dezePlek = plekkenLijst?.FirstOrDefault();
                var bestaandeLijst = dezePlek?["reserveringens"]?.AsArray();

                if (bestaandeLijst != null)
                {
                    foreach (var reservering in bestaandeLijst)
                    {
                        string? bestaandStartStr = reservering["begindatum"]?.ToString();
                        string? bestaandEindStr = reservering["einddatum"]?.ToString();

                        if (bestaandStartStr != null && bestaandEindStr != null)
                        {
                            DateOnly bestaandStart = DateOnly.Parse(bestaandStartStr);
                            DateOnly bestaandEind = DateOnly.Parse(bestaandEindStr);

                            if (nieuwBegin < bestaandEind && nieuwEind > bestaandStart)
                            {
                                Console.WriteLine($"[FOUT] DUBBELE BOEKING! Overlap met {bestaandStart} - {bestaandEind}.");
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"[FOUT] Accommodatie {accommodatieNummer} niet gevonden in backend.");
                return false;
            }

            // --- STAP: PRIJS BEREKENEN ---
            int volwassenen = (int?)(GetCaseInsensitive(json, "Volwassenen")) ?? 0;
            int kind07 = (int?)(GetCaseInsensitive(json, "Kinderen07")) ?? 0;
            int kind712 = (int?)(GetCaseInsensitive(json, "Kinderen712")) ?? 0;

            // Hier roepen we de functie van stap 2 aan
            decimal totaalPrijs = BerekenTotaalPrijs(accommodatieNummer, nieuwBegin, nieuwEind, volwassenen, kind07, kind712);

            Console.WriteLine($"[INFO] Berekende prijs: € {totaalPrijs}");
            // STAP 4: Reservering Versturen
            if (huidigId == 0)
            {
                Console.WriteLine("[FOUT] Geen geldig GebruikerId.");
                return false;
            }

            var payloadObject = new
            {
                Accomodatie = accommodatieNummer,
                GebruikerId = huidigId,
                Begindatum = beginStr,
                Einddatum = eindStr,
                Volwassenen = (int?)(GetCaseInsensitive(json, "Volwassenen")) ?? 0,
                Kinderen07 = (int?)(GetCaseInsensitive(json, "Kinderen07")) ?? 0,
                Kinderen712 = (int?)(GetCaseInsensitive(json, "Kinderen712")) ?? 0,
                TotaalPrijs = totaalPrijs
            };

            var response = await campingClient.PostAsJsonAsync("api/Data/add/reservering", payloadObject);

            if (!response.IsSuccessStatusCode)
            {
                string detail = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[FOUT] Backend weigert reservering ({response.StatusCode}): {detail}");
                return false;
            }

            Console.WriteLine("[SUCCES] Reservering geplaatst.");
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

        // Prijs berekenen
        // De parameters heten nu exact zoals de kolommen in je database tabel
        private decimal BerekenTotaalPrijs(int Accomodatie, DateOnly begindatum, DateOnly einddatum, int volwassenen, int kinderen07, int kinderen712)
        {
            // Bereken aantal nachten
            int nachten = (einddatum.DayNumber - begindatum.DayNumber);
            if (nachten < 1) nachten = 1;

            decimal totaal = 0;

            // LOGICA VOOR CAMPING (Accommodatie nummers 200 en hoger)
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
            // LOGICA VOOR HOTEL (Accommodatie nummers lager dan 200)
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