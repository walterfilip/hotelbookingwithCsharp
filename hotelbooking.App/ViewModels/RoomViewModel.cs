using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using hotelbooking.App.Models;

namespace hotelbooking.App.ViewModels
{
    public partial class RoomViewModel : ObservableObject
    {
        private static readonly HttpClient client = new();

        public ObservableCollection<Room> Rooms { get; } = new();

        [ObservableProperty]
        private string errorMessage = string.Empty;
        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task LoadRoomsAsync()
        {
            IsBusy = true;
            try
            {               
                var rooms = await client.GetFromJsonAsync<List<Room>>($"{ApiConfig.BaseUrl}/room");

                Rooms.Clear();
                foreach (var room in rooms ?? new List<Room>())
                {
                    Rooms.Add(room);
                }
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Rooms.Clear();
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;

            }
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Room? room)
        {
            if (room is null)
                return;
            await Shell.Current.GoToAsync(nameof(RoomDetailPage), new Dictionary<string, object>
            {
                {"Room", room }
            });
        }
    }
}
