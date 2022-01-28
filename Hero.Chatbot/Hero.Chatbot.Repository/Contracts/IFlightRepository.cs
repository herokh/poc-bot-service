using Hero.Chatbot.Domain;
using Hero.Shared.Repository;
using Hero.Chatbot.ViewModel.Flight;
using System.Collections.Generic;

namespace Hero.Chatbot.Repository.Contracts
{
    public interface IFlightRepository : IRepository<Flight>
    {
        IEnumerable<Flight> GetFlights(FlightFilter filter);
    }
}
