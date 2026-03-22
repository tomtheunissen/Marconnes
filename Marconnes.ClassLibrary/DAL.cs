using System;
using System.Collections.Generic;
using Marconnes.ConsoleApp;
using Microsoft.Data.SqlClient;

namespace Marconnes.ConsoleApp
{
    public class DAL
    {
        private readonly string _connectionString = "Data Source=localhost;Initial Catalog=marconnes-db;Integrated Security=True;TrustServerCertificate=True;";

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
                            RoomNumber = (int)reader["RoomNumber"],
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


        // 3. GET ROOM BY ID
        public HotelRoom? GetRoomById(int RoomNumber)
        {
            HotelRoom? room = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM HotelRooms WHERE RoomNumber = @RoomNumber";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomNumber", RoomNumber);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            room = new HotelRoom
                            {
                                RoomNumber = (int)reader["RoomNumber"],
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

        // 4. UPDATE ROOM
        public void UpdateRoom(HotelRoom room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE HotelRooms SET 
                               MaxGuests = @MaxGuests, Price = @Price,
                               Floor = @Floor, SquareMeters = @SquareMeters, NumberOfBeds = @NumberOfBeds, IsDoubleBed = @IsDoubleBed,
                               HasAirConditioning = @HasAirConditioning, HasHeating = @HasHeating, HasWifi = @HasWifi, 
                               HasTelevision = @HasTelevision, IsWheelchairAccessible = @IsWheelchairAccessible, IsSmokingAllowed = @IsSmokingAllowed
                               WHERE RoomNumber = @RoomNumber";

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

        // 5. DELETE ROOM
        public void DeleteRoom(int RoomNumber)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM HotelRooms WHERE RoomID = @RoomNumber";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomNumber", RoomNumber);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}