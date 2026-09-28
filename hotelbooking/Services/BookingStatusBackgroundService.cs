namespace hotelbooking.Services
{
    public class BookingStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public BookingStatusBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    BookingService bookingService = 
                        scope.ServiceProvider.GetRequiredService<BookingService>();

                    bookingService.UpdateExpiredBookings();

                }
                // Perform background tasks here, such as checking booking statuses
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Delay for 1 minute
            }
        }
    }
}
