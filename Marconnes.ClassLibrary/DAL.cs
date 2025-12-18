using System;
using System.Collections.Generic;
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
    }
}