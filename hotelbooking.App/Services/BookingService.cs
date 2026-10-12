using hotelbooking.App.Models;
using System.Net.Http.Json;


namespace hotelbooking.App.Services
{
    public class BookingService
    {
        private static readonly HttpClient client = new();

        public async Task<List<Booking>> GetBookingsAsync()
        {
            var bookings = await client.GetFromJsonAsync<List<Booking>>($"{ApiConfig.BaseUrl}/booking");
            return bookings ?? new List<Booking>();
        }
        public async Task<Booking> CreateBookingAsync(BookingCreateRequest bookingRequest)
        {
            var response = await client.PostAsJsonAsync($"{ApiConfig.BaseUrl}/booking", bookingRequest);
            response.EnsureSuccessStatusCode();

            var booking = await response.Content.ReadFromJsonAsync<Booking>();

            return booking ?? throw new InvalidOperationException("Failed to create booking");
        }
    }
}
