using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using hotelbooking.App.Models;


namespace hotelbooking.App.Services
{
    public class RoomService
    {
        private static readonly HttpClient client = new();
        public async Task<List<Room>> GetRoomsAsync()
        {
            var rooms = await client.GetFromJsonAsync<List<Room>>($"{ ApiConfig.BaseUrl}/room");
            return rooms ?? new List<Room>();
        }
    }
}
