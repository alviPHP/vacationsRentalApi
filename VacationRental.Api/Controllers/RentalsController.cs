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
        private readonly IDictionary<int, PreparationTimeViewModel> _preparationTime;


        public RentalsController(IDictionary<int, RentalViewModel> rentals, 
            IDictionary<int, BookingViewModel> bookings,
            IDictionary<int, PreparationTimeViewModel> preparationTime)
        {
            _rentals = rentals;
            _bookings = bookings;
            _preparationTime = preparationTime;
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
            //int Units = 0;
            //int PreparationTimeInDays = 0;
            if (model.Id < 0)
                    throw new ApplicationException("Id must be positive");
            if (!_rentals.ContainsKey(model.Id))
                throw new ApplicationException("Rental not found");
            
            foreach(var booking in _bookings.Values)
            {
                foreach (var rental in _rentals.Values)
                {
                    if(booking.RentalId == rental.Id)
                    {
                        // If Units are increased
                        if (model.Units > rental.Units)
                        {
                            rental.Units = model.Units;
                        }// If Units are decreased
                        else if (model.Units < rental.Units)
                        {
                            if(_preparationTime[booking.Id].Unit > model.Units)
                                throw new ApplicationException("Units Overlap");
                        }
                        //else
                        //{
                        //    Units = rental.Units;
                        //}
                        
                        // If Preaparation days are increased
                        if (model.PreparationTimeInDays > rental.PreparationTimeInDays)
                        {
                            rental.PreparationTimeInDays = model.PreparationTimeInDays;
                        }// If Preaparation days are decreased
                        else if(model.PreparationTimeInDays < rental.PreparationTimeInDays)
                        {
                          //  
                            if(_preparationTime[booking.Id].Unit > 0)
                            {
                                var xNewEndDate = booking.Start.AddDays(booking.Nights).AddDays(model.PreparationTimeInDays);
                                var xEndDate = booking.Start.AddDays(booking.Nights).AddDays(rental.PreparationTimeInDays);

                                if (booking.Unit == _preparationTime[booking.Id].Unit && xNewEndDate== xEndDate)
                                    throw new ApplicationException("Unit Overlap");
                            }
                        }
                        //else
                        //{
                        //    PreparationTimeInDays = rental.PreparationTimeInDays;
                        //}
                    }
                }
            }

           // _rentals[model.Id].Units = Units;
           // _rentals[model.Id].PreparationTimeInDays = PreparationTimeInDays;            

            return _rentals[model.Id];
        }


    }
}
