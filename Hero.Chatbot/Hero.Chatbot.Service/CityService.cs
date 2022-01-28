using Hero.Chatbot.Domain.City;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;

namespace Hero.Chatbot.Service
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICityRepository _cityRepository;

        public CityService(IUnitOfWork unitOfWork,
            ICityRepository cityRepository)
        {
            _unitOfWork = unitOfWork;
            _cityRepository = cityRepository;
        }

        public IEnumerable<City> GetCities()
        {
            using (_unitOfWork.CreateConnection())
            {
                var cities = _cityRepository.FindAll();
                return cities;
            }
        }
    }
}
