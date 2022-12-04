using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IBookingService _bookingService;

        public BookingsController(IDictionary<int, RentalViewModel> rentals,
                                  IDictionary<int, BookingViewModel> bookings,
                                  IBookingService bookingService)
        {
            _rentals = rentals;
            _bookings = bookings;
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            return _bookingService.Get(bookingId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            return _bookingService.Post(model);
        }
    }
}
