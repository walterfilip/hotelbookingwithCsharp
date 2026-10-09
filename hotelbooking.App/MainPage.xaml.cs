using System.Net.Http.Json;
using hotelbooking.App.Models;
using hotelbooking.App.ViewModels;

namespace hotelbooking.App
{
    public partial class MainPage : ContentPage
    {
        private readonly RoomViewModel viewModel = new();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.LoadRoomsCommand.ExecuteAsync(null);
        }
    }
}
