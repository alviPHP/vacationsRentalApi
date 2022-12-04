using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IDictionary<int, PreparationTimeViewModel> _preparationTime;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings,
            IDictionary<int, PreparationTimeViewModel> preparationTime)
        {
            _rentals = rentals;
            _bookings = bookings;
            _preparationTime= preparationTime;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            if (!_bookings.ContainsKey(bookingId))
                throw new ApplicationException("Booking not found");

            return _bookings[bookingId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentals.ContainsKey(model.RentalId))
                throw new ApplicationException("Rental not found");

            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in _bookings.Values)
                {
                    if (booking.RentalId == model.RentalId)
                    {
                        //Get End Date
                        var endDate1 = booking.Start.AddDays(booking.Nights);
                        var endDate2 = model.Start.AddDays(model.Nights);

                        //Adding X additional days for cleaning on the day end.
                        var xEndDate1 = endDate1.AddDays(_rentals[booking.RentalId].PreparationTimeInDays);
                        var xEndDate2 = endDate2.AddDays(_rentals[model.RentalId].PreparationTimeInDays);

                        //Check if unit is occupied
                        if ((booking.Start <= model.Start.Date && xEndDate1 > model.Start.Date) 
                            || (booking.Start < xEndDate2 && xEndDate1 >= xEndDate2) || (booking.Start > model.Start && xEndDate1 < xEndDate2))
                         {
                            //Check if same unit occupied for the night 
                            if(booking.Unit == model.Unit)
                                throw new ApplicationException("Not available");

                            //Add PreparationTime units
                            if (model.Start > endDate1 && model.Start <= xEndDate1)
                            {
                                if(!_preparationTime.ContainsKey(booking.Id))
                                {
                                    _preparationTime.Add(booking.Id, new PreparationTimeViewModel
                                    {
                                        Unit = booking.Unit
                                    });
                                }                                
                            }
                            // Count occupied unit
                            count++;
                         }                      
                    }
                }
                if (count >= _rentals[model.RentalId].Units)
                    throw new ApplicationException("Not available");
            }

            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Unit = model.Unit                
            });

            return key;
        }
    }
}
