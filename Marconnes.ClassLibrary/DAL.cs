using System;
using System.Collections.Generic;
using Marconnes.ConsoleApp;
using Microsoft.Data.SqlClient;

namespace Marconnes.ConsoleApp
{
    public class DAL
    {
        private readonly string _connectionString =
        "Data Source=marconnes-db.database.windows.net;Initial Catalog=Marconnes;Persist Security Info=True;User ID=projectadmin;Password=Goed-W8achtwoord;Trust Server Certificate=True";

        // 1. GET ALL ROOMS
        public List<HotelRoom> GetAllRooms()
        {
            var rooms = new List<HotelRoom>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT RoomID, RoomNumber, MaxGuests, Price FROM HotelRooms";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new HotelRoom
                        {
                            RoomID = reader.GetInt32(0),
                            RoomNumber = reader.GetString(1),
                            MaxGuests = reader.GetInt32(2),
                            Price = reader.GetDecimal(3)
                        });
                    }
                }
            }

            return rooms;
        }
        public List<CampingPlace> GetAllPlaces()
        {
            var Places = new List<CampingPlace>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT PlaceID, PlaceNumber, MaxGuests, Price FROM CampingPlaces";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Places.Add(new CampingPlace
                        {
                            PlaceID = reader.GetInt32(0),
                            PlaceNumber = reader.GetString(1),
                            MaxGuests = reader.GetInt32(2),
                            Price = reader.GetDecimal(3)
                        });
                    }
                }
            }

            return Places;
        }


        // 2. ADD ROOM
        public void AddHotelRoom(HotelRoom room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO HotelRooms (RoomNumber, MaxGuests, Price) VALUES (@RoomNumber, @MaxGuests, @Price)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {

                    cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", room.Price);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddCampingPlace(CampingPlace Place)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO CampingPlaces (PlaceNumber, MaxGuests, Price) VALUES (@PlaceNumber, @MaxGuests, @Price)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {

                    cmd.Parameters.AddWithValue("@PlaceNumber", Place.PlaceNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", Place.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", Place.Price);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. GET ROOM BY ID
        public HotelRoom? GetRoomById(int id)
        {
            HotelRoom? room = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT RoomID, RoomNumber, MaxGuests, Price FROM HotelRooms WHERE RoomID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            room = new HotelRoom
                            {
                                RoomID = reader.GetInt32(0),
                                RoomNumber = reader.GetString(1),
                                MaxGuests = reader.GetInt32(2),
                                Price = reader.GetDecimal(3)
                            };
                        }
                    }
                }
            }

            return room;
        }
        public CampingPlace? GetPlaceById(int id)
        {
            CampingPlace? Place = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT PlaceID, PlaceNumber, MaxGuests, Price FROM CampingPlaces WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Place = new CampingPlace
                            {
                                PlaceID = reader.GetInt32(0),
                                PlaceNumber = reader.GetString(1),
                                MaxGuests = reader.GetInt32(2),
                                Price = reader.GetDecimal(3)
                            };
                        }
                    }
                }
            }

            return Place;
        }

        // 4. UPDATE ROOM
        public void UpdateRoom(HotelRoom room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE HotelRooms SET RoomNumber = @RoomNumber, MaxGuests = @MaxGuests, Price = @Price WHERE RoomID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", room.Price);
                    cmd.Parameters.AddWithValue("@Id", room.RoomID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdatePlace(CampingPlace Place)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE Campingplace SET PlaceNumber = @PlaceNumber, MaxGuests = @MaxGuests, Price = @Price WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PlaceNumber", Place.PlaceNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", Place.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", Place.Price);
                    cmd.Parameters.AddWithValue("@Id", Place.PlaceID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 5. DELETE ROOM
        public void DeleteRoom(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM HotelRooms WHERE RoomID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeletePlace(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM CampingPlace WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}