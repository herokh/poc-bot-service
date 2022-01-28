using System;

namespace Hero.Chatbot.Web.Models
{
    public class FlightSearchCriteriaViewModel
    {
        public string Origin { get; set; }

        public string Destination { get; set; }

        public DateTime? DepartureDate { get; set; }

        public DateTime? ArrivalDate { get; set; }
    }
}
