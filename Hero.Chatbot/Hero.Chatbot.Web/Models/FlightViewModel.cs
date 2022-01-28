using Hero.Chatbot.Web.Context;
using System;
using System.Text;

namespace Hero.Chatbot.Web.Models
{
    public class FlightViewModel
    {
        public int FlightId { get; set; }

        public string FlightNumber { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public int DurationHours { get; set; }

        public int DurationMinutes { get; set; }

        public decimal Amount { get; set; }

        public string FormattedPrice
        {
            get
            {
                return string.Format(ApplicationContext.CurrentCultureInfo, "{0:C}", Amount);
            }
        }

        public string FormattedDepartureDateTime
        {
            get
            {
                return DepartureDateTime.ToString("dd MMMM yyyy");
            }
        }

        public string FormattedArrivalDateTime
        {
            get
            {
                return ArrivalDateTime.ToString("dd MMMM yyyy");
            }
        }

        public string FormattedTotalDuration
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"{DurationHours} hr");
                if (DurationMinutes > 0)
                {
                    sb.Append($" {DurationMinutes} min");
                }
                return sb.ToString();
            }
        }

        public string FormattedPeriod
        {
            get
            {
                var departure = DepartureDateTime.ToString("HH:mm", ApplicationContext.CurrentCultureInfo);
                var arrival = ArrivalDateTime.ToString("HH:mm", ApplicationContext.CurrentCultureInfo);
                return $"{departure} - {arrival}";
            }
        }
    }
}
