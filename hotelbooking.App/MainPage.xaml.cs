using hotelbooking.App.ViewModels;

namespace hotelbooking.App
{
    public partial class MainPage : ContentPage
    {
        private readonly RoomViewModel viewModel;
        public MainPage(RoomViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }
       
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            RoomView.SelectedItem = null;
            await viewModel.LoadRoomsCommand.ExecuteAsync(null);
        }
    }
}
