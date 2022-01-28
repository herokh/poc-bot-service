using System;
using System.Collections.Generic;

namespace Hero.Chatbot.Web.Models
{
    public class FlightSearchViewModel
    {
        public string Origin { get; set; }

        public string Destination { get; set; }

        public DateTime? DepartureDate { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public IEnumerable<AutocompleteViewModel> Cities { get; set; }
    }
}
