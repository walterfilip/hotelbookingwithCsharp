using hotelbooking.App.ViewModels;

namespace hotelbooking.App;

public partial class RoomDetailPage : ContentPage
{
	public RoomDetailPage()
	{
		InitializeComponent();
        BindingContext = new RoomDetailViewModel();
    }
}