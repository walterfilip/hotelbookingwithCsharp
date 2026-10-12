using hotelbooking.App.ViewModels;
using hotelbooking.App.Services;

using Microsoft.Extensions.Logging;

namespace hotelbooking.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {            


            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<RoomService>();
            builder.Services.AddTransient<RoomViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<BookingService>();
            builder.Services.AddTransient<BookingPage>();
            builder.Services.AddTransient<BookingViewModel>();
            builder.Services.AddTransient<CreateBookingPage>();
            builder.Services.AddTransient<CreateBookingViewModel>();

            return builder.Build();
        }
    }
}
