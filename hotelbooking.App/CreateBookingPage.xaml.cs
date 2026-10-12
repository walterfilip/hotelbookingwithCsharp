using hotelbooking.App.ViewModels;

namespace hotelbooking.App;

public partial class CreateBookingPage : ContentPage
{
	public CreateBookingPage(CreateBookingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}