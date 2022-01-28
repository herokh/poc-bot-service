using Hero.Chatbot.Domain;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.UnitOfWork;
using Hero.Chatbot.ViewModel.Flight;
using System.Collections.Generic;

namespace Hero.Chatbot.Service
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IFlightRepository _flightRepository;

        public FlightService(IUnitOfWork unitOfWork,
            IFlightRepository flightRepository)
        {
            _unitOfWork = unitOfWork;
            _flightRepository = flightRepository;
        }

        public IEnumerable<Flight> GetFlights(FlightFilter filter)
        {
            using (_unitOfWork.CreateConnection())
            {
                _unitOfWork.Create();
                var result = _flightRepository.GetFlights(filter);
                _unitOfWork.Commit();
                return result;
            }
        }
    }
}
