using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;


        public RentalsController(IDictionary<int, RentalViewModel> rentals, 
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });

            return key;
        }

        [HttpPut]
        public RentalViewModel Put(RentalUpdateBindingModel model)
        {
            if (model.Id < 0)
                throw new ApplicationException("Id must be positive");
            if (!_rentals.ContainsKey(model.Id))
                throw new ApplicationException("Rental not found"); 
            if(model.Units <= 0)
                throw new ApplicationException("Units must be greater then 0");
            if (model.PreparationTimeInDays <= 0)
                throw new ApplicationException("Preparation time must be greater then 0");


            foreach (var booking in _bookings.Values)
            {
                if (booking.RentalId == model.Id)
                {
                    //Check Overlap.

                    if (model.Units < _rentals[model.Id].Units 
                        && booking.Unit > model.Units)
                    {
                        throw new ApplicationException("Units Overlap..");
                    }
                    
                    if (model.PreparationTimeInDays < _rentals[model.Id].PreparationTimeInDays
                        && (DateTime.Today > booking.Start.AddDays(booking.Nights)
                        && DateTime.Today <= booking.Start.AddDays(booking.Nights).AddDays(_rentals[booking.RentalId].PreparationTimeInDays) 
                        && booking.Unit > 0))

                     {
                         throw new ApplicationException("A Unit Overlap with preparation day time.");
                     }
                }
                                
            }

            _rentals[model.Id].Units = model.Units;
            _rentals[model.Id].PreparationTimeInDays = model.PreparationTimeInDays;

            return _rentals[model.Id];
        }
    }
}
