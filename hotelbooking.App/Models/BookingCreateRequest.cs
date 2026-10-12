using hotelbooking.App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hotelbooking.App.Models
{
    public class BookingCreateRequest
    {
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
    }
}
