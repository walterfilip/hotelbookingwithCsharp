using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using hotelbooking.App.Models;
using hotelbooking.App.Services;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace hotelbooking.App.ViewModels
{
    public partial class BookingViewModel : ObservableObject
    {
        private readonly BookingService bookingService;

        public BookingViewModel(BookingService bookingService)
        {
            this.bookingService = bookingService;
        }
        public ObservableCollection<Booking> Bookings { get; } = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            IsBusy = true;

            try
            {
                var bookings = await bookingService.GetBookingsAsync();

                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    Bookings.Add(booking);
                }
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Bookings.Clear();
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
