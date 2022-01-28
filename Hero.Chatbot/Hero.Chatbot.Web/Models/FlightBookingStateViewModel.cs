using Hero.Chatbot.Web.Context;
using Hero.Chatbot.Web.Enums;
using System;

namespace Hero.Chatbot.Web.Models
{
    public class FlightBookingStateViewModel
    {
        public EnumFlightBookingStep CurrentStep { get; set; }

        public EnumFlightBookingStep NextStep { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public decimal DepartureAmount { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public decimal ArrivalAmount { get; set; }

        public PassengerInfoViewModel Passenger { get; set; }

        public PaymentInfoViewModel Payment { get; set; }

        public string TotalAmount
        {
            get
            {
                return string.Format(ApplicationContext.CurrentCultureInfo, "{0:C}", DepartureAmount + ArrivalAmount);
            }
        }

        public string FormattedDepartureDateTime
        {
            get
            {
                return DepartureDateTime.ToString("dd MMMM yyyy hh:mm:ss", ApplicationContext.CurrentCultureInfo);
            }
        }

        public string FormattedArrivalDateTime
        {
            get
            {
                return ArrivalDateTime.ToString("dd MMMM yyyy hh:mm:ss", ApplicationContext.CurrentCultureInfo);
            }
        }
    }
}
