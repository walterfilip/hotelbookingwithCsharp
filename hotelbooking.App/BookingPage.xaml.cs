using hotelbooking.App.ViewModels;

namespace hotelbooking.App;

public partial class BookingPage : ContentPage
{
	private readonly BookingViewModel viewModel;
    public BookingPage(BookingViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadBookingsCommand.ExecuteAsync(null);

    }
}