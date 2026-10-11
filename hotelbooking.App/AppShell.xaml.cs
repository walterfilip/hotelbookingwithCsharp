namespace hotelbooking.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RoomDetailPage), typeof(RoomDetailPage));
        }
    }
}
