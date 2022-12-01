using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CalendarBookingViewModel
    {
        public int Id { get; set; }

        public int Unit { get; set; }

        public List<PreparationTimeViewModel> PreparationTimes { get; set; }
        
    }
}
