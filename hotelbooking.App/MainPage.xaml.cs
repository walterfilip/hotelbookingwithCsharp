using System.Net.Http.Json;
using hotelbooking.App.Models;
namespace hotelbooking.App
{
    public partial class MainPage : ContentPage
    {
       
        private static readonly HttpClient client = new();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
         try
            {
                var rooms = await client.GetFromJsonAsync<List<Room>>("http://localhost:8080/api/room");
                Roomsview.ItemsSource = rooms;

                ResultLabel.Text = "";
                   // string.Join("\n",
                   // (rooms ?? new List<Room>()).Select(r => $"Rum {r.RoomNumber} ({r.Type}) - {r.Price} kr"));
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Fel: {ex.Message}";
            }
        }
    }
}
