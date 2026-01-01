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
                string sql = "SELECT * FROM HotelRooms";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new HotelRoom
                        {
                            RoomID = (int)reader["RoomID"],
                            RoomNumber = reader["RoomNumber"].ToString(),
                            MaxGuests = (int)reader["MaxGuests"],
                            Price = (decimal)reader["Price"],
                            Floor = reader["Floor"] != DBNull.Value ? (int)reader["Floor"] : 0,
                            SquareMeters = reader["SquareMeters"] != DBNull.Value ? (int)reader["SquareMeters"] : 0,
                            NumberOfBeds = reader["NumberOfBeds"] != DBNull.Value ? (int)reader["NumberOfBeds"] : 0,
                            IsDoubleBed = reader["IsDoubleBed"] != DBNull.Value ? (bool)reader["IsDoubleBed"] : false,
                            HasAirConditioning = reader["HasAirConditioning"] != DBNull.Value ? (bool)reader["HasAirConditioning"] : false,
                            HasHeating = reader["HasHeating"] != DBNull.Value ? (bool)reader["HasHeating"] : false,
                            HasWifi = reader["HasWifi"] != DBNull.Value ? (bool)reader["HasWifi"] : false,
                            HasTelevision = reader["HasTelevision"] != DBNull.Value ? (bool)reader["HasTelevision"] : false,
                            IsWheelchairAccessible = reader["IsWheelchairAccessible"] != DBNull.Value ? (bool)reader["IsWheelchairAccessible"] : false,
                            IsSmokingAllowed = reader["IsSmokingAllowed"] != DBNull.Value ? (bool)reader["IsSmokingAllowed"] : false
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
                string sql = "SELECT * FROM CampingPlaces";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Places.Add(new CampingPlace
                        {
                            PlaceID = (int)reader["PlaceID"],
                            PlaceNumber = reader["PlaceNumber"].ToString(),
                            MaxGuests = (int)reader["MaxGuests"],
                            Price = (decimal)reader["Price"],
                            HasElectricity = reader["HasElectricity"] != DBNull.Value ? (bool)reader["HasElectricity"] : false,
                            Ampere = reader["Ampere"] != DBNull.Value ? (int)reader["Ampere"] : 0,
                            HasWaterConnection = reader["HasWaterConnection"] != DBNull.Value ? (bool)reader["HasWaterConnection"] : false,
                            HasSewageDrain = reader["HasSewageDrain"] != DBNull.Value ? (bool)reader["HasSewageDrain"] : false,
                            SurfaceArea = reader["SurfaceArea"] != DBNull.Value ? (int)reader["SurfaceArea"] : 0,
                            GroundType = reader["GroundType"] != DBNull.Value ? reader["GroundType"].ToString() : "",
                            IsShaded = reader["IsShaded"] != DBNull.Value ? (bool)reader["IsShaded"] : false,
                            IsCarAllowed = reader["IsCarAllowed"] != DBNull.Value ? (bool)reader["IsCarAllowed"] : false,
                            ArePetsAllowed = reader["ArePetsAllowed"] != DBNull.Value ? (bool)reader["ArePetsAllowed"] : false
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
                string sql = @"INSERT INTO HotelRooms 
                               (RoomNumber, MaxGuests, Price, Floor, SquareMeters, NumberOfBeds, IsDoubleBed, HasAirConditioning, HasHeating, HasWifi, HasTelevision, IsWheelchairAccessible, IsSmokingAllowed) 
                               VALUES 
                               (@RoomNumber, @MaxGuests, @Price, @Floor, @SquareMeters, @NumberOfBeds, @IsDoubleBed, @HasAirConditioning, @HasHeating, @HasWifi, @HasTelevision, @IsWheelchairAccessible, @IsSmokingAllowed)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", room.Price);
                    cmd.Parameters.AddWithValue("@Floor", room.Floor);
                    cmd.Parameters.AddWithValue("@SquareMeters", room.SquareMeters);
                    cmd.Parameters.AddWithValue("@NumberOfBeds", room.NumberOfBeds);
                    cmd.Parameters.AddWithValue("@IsDoubleBed", room.IsDoubleBed);
                    cmd.Parameters.AddWithValue("@HasAirConditioning", room.HasAirConditioning);
                    cmd.Parameters.AddWithValue("@HasHeating", room.HasHeating);
                    cmd.Parameters.AddWithValue("@HasWifi", room.HasWifi);
                    cmd.Parameters.AddWithValue("@HasTelevision", room.HasTelevision);
                    cmd.Parameters.AddWithValue("@IsWheelchairAccessible", room.IsWheelchairAccessible);
                    cmd.Parameters.AddWithValue("@IsSmokingAllowed", room.IsSmokingAllowed);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddCampingPlace(CampingPlace Place)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO CampingPlaces 
                               (PlaceNumber, MaxGuests, Price, HasElectricity, Ampere, HasWaterConnection, HasSewageDrain, SurfaceArea, GroundType, IsShaded, IsCarAllowed, ArePetsAllowed) 
                               VALUES 
                               (@PlaceNumber, @MaxGuests, @Price, @HasElectricity, @Ampere, @HasWaterConnection, @HasSewageDrain, @SurfaceArea, @GroundType, @IsShaded, @IsCarAllowed, @ArePetsAllowed)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PlaceNumber", Place.PlaceNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", Place.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", Place.Price);
                    cmd.Parameters.AddWithValue("@HasElectricity", Place.HasElectricity);
                    cmd.Parameters.AddWithValue("@Ampere", Place.Ampere);
                    cmd.Parameters.AddWithValue("@HasWaterConnection", Place.HasWaterConnection);
                    cmd.Parameters.AddWithValue("@HasSewageDrain", Place.HasSewageDrain);
                    cmd.Parameters.AddWithValue("@SurfaceArea", Place.SurfaceArea);
                    cmd.Parameters.AddWithValue("@GroundType", (object)Place.GroundType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsShaded", Place.IsShaded);
                    cmd.Parameters.AddWithValue("@IsCarAllowed", Place.IsCarAllowed);
                    cmd.Parameters.AddWithValue("@ArePetsAllowed", Place.ArePetsAllowed);

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
                string sql = "SELECT * FROM HotelRooms WHERE RoomID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            room = new HotelRoom
                            {
                                RoomID = (int)reader["RoomID"],
                                RoomNumber = reader["RoomNumber"].ToString(),
                                MaxGuests = (int)reader["MaxGuests"],
                                Price = (decimal)reader["Price"],
                                Floor = reader["Floor"] != DBNull.Value ? (int)reader["Floor"] : 0,
                                SquareMeters = reader["SquareMeters"] != DBNull.Value ? (int)reader["SquareMeters"] : 0,
                                NumberOfBeds = reader["NumberOfBeds"] != DBNull.Value ? (int)reader["NumberOfBeds"] : 0,
                                IsDoubleBed = reader["IsDoubleBed"] != DBNull.Value ? (bool)reader["IsDoubleBed"] : false,
                                HasAirConditioning = reader["HasAirConditioning"] != DBNull.Value ? (bool)reader["HasAirConditioning"] : false,
                                HasHeating = reader["HasHeating"] != DBNull.Value ? (bool)reader["HasHeating"] : false,
                                HasWifi = reader["HasWifi"] != DBNull.Value ? (bool)reader["HasWifi"] : false,
                                HasTelevision = reader["HasTelevision"] != DBNull.Value ? (bool)reader["HasTelevision"] : false,
                                IsWheelchairAccessible = reader["IsWheelchairAccessible"] != DBNull.Value ? (bool)reader["IsWheelchairAccessible"] : false,
                                IsSmokingAllowed = reader["IsSmokingAllowed"] != DBNull.Value ? (bool)reader["IsSmokingAllowed"] : false
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
                string sql = "SELECT * FROM CampingPlaces WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Place = new CampingPlace
                            {
                                PlaceID = (int)reader["PlaceID"],
                                PlaceNumber = reader["PlaceNumber"].ToString(),
                                MaxGuests = (int)reader["MaxGuests"],
                                Price = (decimal)reader["Price"],
                                HasElectricity = reader["HasElectricity"] != DBNull.Value ? (bool)reader["HasElectricity"] : false,
                                Ampere = reader["Ampere"] != DBNull.Value ? (int)reader["Ampere"] : 0,
                                HasWaterConnection = reader["HasWaterConnection"] != DBNull.Value ? (bool)reader["HasWaterConnection"] : false,
                                HasSewageDrain = reader["HasSewageDrain"] != DBNull.Value ? (bool)reader["HasSewageDrain"] : false,
                                SurfaceArea = reader["SurfaceArea"] != DBNull.Value ? (int)reader["SurfaceArea"] : 0,
                                GroundType = reader["GroundType"] != DBNull.Value ? reader["GroundType"].ToString() : "",
                                IsShaded = reader["IsShaded"] != DBNull.Value ? (bool)reader["IsShaded"] : false,
                                IsCarAllowed = reader["IsCarAllowed"] != DBNull.Value ? (bool)reader["IsCarAllowed"] : false,
                                ArePetsAllowed = reader["ArePetsAllowed"] != DBNull.Value ? (bool)reader["ArePetsAllowed"] : false
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
                string sql = @"UPDATE HotelRooms SET 
                               RoomNumber = @RoomNumber, MaxGuests = @MaxGuests, Price = @Price,
                               Floor = @Floor, SquareMeters = @SquareMeters, NumberOfBeds = @NumberOfBeds, IsDoubleBed = @IsDoubleBed,
                               HasAirConditioning = @HasAirConditioning, HasHeating = @HasHeating, HasWifi = @HasWifi, 
                               HasTelevision = @HasTelevision, IsWheelchairAccessible = @IsWheelchairAccessible, IsSmokingAllowed = @IsSmokingAllowed
                               WHERE RoomID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", room.Price);
                    cmd.Parameters.AddWithValue("@Floor", room.Floor);
                    cmd.Parameters.AddWithValue("@SquareMeters", room.SquareMeters);
                    cmd.Parameters.AddWithValue("@NumberOfBeds", room.NumberOfBeds);
                    cmd.Parameters.AddWithValue("@IsDoubleBed", room.IsDoubleBed);
                    cmd.Parameters.AddWithValue("@HasAirConditioning", room.HasAirConditioning);
                    cmd.Parameters.AddWithValue("@HasHeating", room.HasHeating);
                    cmd.Parameters.AddWithValue("@HasWifi", room.HasWifi);
                    cmd.Parameters.AddWithValue("@HasTelevision", room.HasTelevision);
                    cmd.Parameters.AddWithValue("@IsWheelchairAccessible", room.IsWheelchairAccessible);
                    cmd.Parameters.AddWithValue("@IsSmokingAllowed", room.IsSmokingAllowed);
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
                string sql = @"UPDATE CampingPlaces SET 
                               PlaceNumber = @PlaceNumber, MaxGuests = @MaxGuests, Price = @Price,
                               HasElectricity = @HasElectricity, Ampere = @Ampere, HasWaterConnection = @HasWaterConnection, 
                               HasSewageDrain = @HasSewageDrain, SurfaceArea = @SurfaceArea, GroundType = @GroundType, 
                               IsShaded = @IsShaded, IsCarAllowed = @IsCarAllowed, ArePetsAllowed = @ArePetsAllowed
                               WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PlaceNumber", Place.PlaceNumber);
                    cmd.Parameters.AddWithValue("@MaxGuests", Place.MaxGuests);
                    cmd.Parameters.AddWithValue("@Price", Place.Price);
                    cmd.Parameters.AddWithValue("@HasElectricity", Place.HasElectricity);
                    cmd.Parameters.AddWithValue("@Ampere", Place.Ampere);
                    cmd.Parameters.AddWithValue("@HasWaterConnection", Place.HasWaterConnection);
                    cmd.Parameters.AddWithValue("@HasSewageDrain", Place.HasSewageDrain);
                    cmd.Parameters.AddWithValue("@SurfaceArea", Place.SurfaceArea);
                    cmd.Parameters.AddWithValue("@GroundType", (object)Place.GroundType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsShaded", Place.IsShaded);
                    cmd.Parameters.AddWithValue("@IsCarAllowed", Place.IsCarAllowed);
                    cmd.Parameters.AddWithValue("@ArePetsAllowed", Place.ArePetsAllowed);
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
                string sql = "DELETE FROM CampingPlaces WHERE PlaceID = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}