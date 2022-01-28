using Hero.Chatbot.Domain;
using Hero.Chatbot.ViewModel.Flight;

namespace Hero.Chatbot.BotService.Models
{
    public class FlightSearchModel
    {
        public string Origin { get; set; }

        public string Destination { get; set; }

        public string DepartureDate { get; set; }

        public FlightFilter ToFlightFilter()
        {
            var filter = new FlightFilter
            {
                Origin = this.Origin,
                Destination = this.Destination,
                DepartureDate = this.DepartureDate
            };

            return filter;
        }
    }
}
