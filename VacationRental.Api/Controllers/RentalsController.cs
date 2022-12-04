using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IRentalService _rentalService;


        public RentalsController(IDictionary<int, RentalViewModel> rentals, 
            IDictionary<int, BookingViewModel> bookings, IRentalService rentalService)
        {
            _rentals = rentals;
            _bookings = bookings;
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            return _rentalService.Get(rentalId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            return _rentalService.Post(model);
        }

        [HttpPut]
        public RentalViewModel Put(RentalUpdateBindingModel model)
        {
            return _rentalService.Put(model);
        }
    }
}
