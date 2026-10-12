using System;
using System.Collections.Generic;
using System.Text;

namespace hotelbooking.App.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerFirstNameSnapshot { get; set; } = string.Empty;
        public string CustomerLastNameSnapshot { get; set; } = string.Empty;
        public string CustomerEmailSnapshot { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public string CustomerFullname => $"{CustomerFirstNameSnapshot} {CustomerLastNameSnapshot}";
    }
}
