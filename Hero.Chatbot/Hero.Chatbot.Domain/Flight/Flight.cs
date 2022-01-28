using Hero.Shared.Repository;
using System;

namespace Hero.Chatbot.Domain
{
    public class Flight : IAggregateRoot
    {
        public int FlightId { get; set; }

        public int AirlineId { get; set; }

        public string AirlineName { get; set; }

        public string FlightNumber { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public string DepartureIataCode { get; set; }

        public string ArrivalIataCode { get; set; }

        public string DepartureCityName { get; set; }

        public string ArrivalCityName { get; set; }

        public string DepartureAirportName { get; set; }

        public string ArrivalAirportName { get; set; }

        public decimal Duration { get; set; }

        public int TotalSeats { get; set; }

        public int DurationHours
        {
            get
            {
                return (int)Math.Round(Duration);
            }
        }

        public int DurationMinutes
        {
            get
            {
                return (int)(60 * (Duration - Math.Round(Duration)));
            }
        }
    }
}
