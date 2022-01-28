using Hero.Chatbot.Domain.City;
using System.Collections.Generic;

namespace Hero.Chatbot.Service.Contracts
{
    public interface ICityService
    {
        IEnumerable<City> GetCities();
    }
}
