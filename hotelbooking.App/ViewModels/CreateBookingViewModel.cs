using hotelbooking.App.Models;
using hotelbooking.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;


namespace hotelbooking.App.ViewModels
{
    public partial class CreateBookingViewModel : ObservableObject
    {
        private readonly BookingService _bookingService;

        public CreateBookingViewModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [ObservableProperty]
        private string customerId = string.Empty;

        [ObservableProperty]
        private string roomId = string.Empty;

        [ObservableProperty]
        private DateTime checkIn = DateTime.Today;

        [ObservableProperty]
        private DateTime checkOut = DateTime.Today.AddDays(1);

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string message = string.Empty;

        [RelayCommand]
        private async Task CreateBookingAsync()
        {
            if (!int.TryParse(CustomerId, out var customerIdValue) || !int.TryParse(RoomId, out var roomIdValue))
            {
                Message = "Kund-ID och Rum-ID måste vara siffror.";
                return;
            }
            if (CheckOut <= CheckIn)
            {
                Message = "Utcheckning måste vara efter incheckning.";
                return;
            }
            isBusy = true;

            try
            {
                var request = new BookingCreateRequest
                {
                    CustomerId = customerIdValue,
                    RoomId = roomIdValue,
                    CheckIn = DateOnly.FromDateTime(CheckIn),
                    CheckOut = DateOnly.FromDateTime(CheckOut)
                };
                var booking = await _bookingService.CreateBookingAsync(request);
                Message = $"Bokning {booking.Id} skapad. Totalt: {booking.TotalPrice} kr";
            }
            catch (Exception ex)
            {
                Message = $"Fel vid skapande av bokning: {ex.Message}";
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
