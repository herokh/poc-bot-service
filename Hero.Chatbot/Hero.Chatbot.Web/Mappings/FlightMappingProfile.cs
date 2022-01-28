using Hero.Chatbot.Domain;
using Hero.Chatbot.Web.Models;
using System;

namespace Hero.Chatbot.Web.Mappings
{
    public static class FlightMappingProfile
    {
        public static FlightViewModel ToView(this Flight flight)
        {
            return new FlightViewModel
            {
                FlightId = flight.FlightId,
                Origin = flight.DepartureIataCode,
                Destination = flight.ArrivalIataCode,
                DurationHours = flight.DurationHours,
                DurationMinutes = flight.DurationMinutes,
                ArrivalDateTime = flight.ArrivalTime,
                DepartureDateTime = flight.DepartureTime,
                FlightNumber = flight.FlightNumber,
                Amount = new Random().Next(1000, 2000)
            };
        }
    }
}
