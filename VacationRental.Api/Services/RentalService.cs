using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public interface IRentalService
    {
        RentalViewModel Get(int rentalId);
        ResourceIdViewModel Post(RentalBindingModel model);
        RentalViewModel Put(RentalUpdateBindingModel model);
    }
    public class RentalService : IRentalService
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public RentalService(IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }
        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

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

        public RentalViewModel Put(RentalUpdateBindingModel model)
        {
            if (model.Id < 0)
                throw new ApplicationException("Id must be positive");
            if (!_rentals.ContainsKey(model.Id))
                throw new ApplicationException("Rental not found");
            if (model.Units <= 0)
                throw new ApplicationException("Units must be greater then 0");
            if (model.PreparationTimeInDays <= 0)
                throw new ApplicationException("Preparation time must be greater then 0");

            foreach (var booking in _bookings.Values)
            {
                if (booking.RentalId == model.Id)
                {
                    /*Check Units Overlap.If Units decrease
                     * 
                     * logic : If current booking unit number is greater then given model units
                     *         then overlap.
                     */

                    if (model.Units < _rentals[model.Id].Units && booking.Unit > model.Units)
                        throw new ApplicationException("Units Overlap..");

                    /*Check preparation time is Overlap.If preparation time is increase
                     * 
                     * logic : If current bookings new x end date (booking nights + new preparation days) 
                     *         is equal to the start date of the same rental unit of following booking
                     *         then overlap.
                     *       
                     */

                    if (model.PreparationTimeInDays > _rentals[model.Id].PreparationTimeInDays)
                    {
                        var newXEndDate = booking.Start.AddDays(booking.Nights).AddDays(model.PreparationTimeInDays);

                        foreach (var nBooking in _bookings.Values)
                        {
                            if (nBooking.RentalId == booking.RentalId 
                                && nBooking.Unit == booking.Unit 
                                && nBooking.Start == newXEndDate)
                            {
                                throw new ApplicationException("A Unit Overlap with preparation day time.");
                            }                                
                        }
                    }
                }
            }

            _rentals[model.Id].Units = model.Units;
            _rentals[model.Id].PreparationTimeInDays = model.PreparationTimeInDays;

            return _rentals[model.Id];
        }
    }
}
