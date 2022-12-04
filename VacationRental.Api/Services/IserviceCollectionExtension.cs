using Microsoft.Extensions.DependencyInjection;

namespace VacationRental.Api.Services
{
    public static class IserviceCollectionExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddScoped<IRentalService, RentalService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ICalendarService, CalendarService>();
            return services;
        }
    }
}
