using Hero.Chatbot.Domain;
using Hero.Chatbot.ViewModel.Flight;
using System.Collections.Generic;

namespace Hero.Chatbot.Service.Contracts
{
    public interface IFlightService
    {
        IEnumerable<Flight> GetFlights(FlightFilter filter);
    }
}
