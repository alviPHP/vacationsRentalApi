using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public interface ICalendarService
    {
        CalendarViewModel Get(int rentalId, DateTime start, int nights);
    }
    public class CalendarService : ICalendarService
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarService(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                foreach (var booking in _bookings.Values)
                {
                    var endDate = booking.Start.AddDays(booking.Nights);
                    var xEndDate = booking.Start.AddDays(booking.Nights).AddDays(_rentals[booking.RentalId].PreparationTimeInDays);

                    if (booking.RentalId == rentalId)
                    {
                        // Add All bookings that exist beyond the given date
                        if (booking.Start <= date.Date && endDate > date.Date)
                        {
                            date.Bookings.Add(new CalendarBookingViewModel
                            {
                                Id = booking.Id,
                                Unit = booking.Unit
                            });
                        }
                        else if (date.Date > endDate && date.Date <= xEndDate)
                        {
                            //Add unit in return object.
                            date.PreparationTimes.Add(new CalendarPreparationTimeViewModel
                            {
                                Unit = booking.Unit
                            });
                        }
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
