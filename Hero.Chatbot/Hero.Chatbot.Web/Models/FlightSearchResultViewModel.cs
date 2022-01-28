using Hero.Chatbot.Web.Context;
using System;
using System.Collections.Generic;

namespace Hero.Chatbot.Web.Models
{
    public class FlightSearchResultViewModel
    {
        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public IEnumerable<FlightViewModel> Flights { get; set; }

        public string FormattedDepartureDate
        {
            get
            {
                return DepartureDate.ToString("dd MMMM yyyy", ApplicationContext.CurrentCultureInfo);
            }
        }

        public string FormattedArrivalDate
        {
            get
            {
                return ArrivalDate.ToString("dd MMMM yyyy", ApplicationContext.CurrentCultureInfo);
            }
        }
    }
}
