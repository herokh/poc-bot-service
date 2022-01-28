using Hero.Shared.Repository;

namespace Hero.Chatbot.Domain.City
{
    public class City : IAggregateRoot
    {
        public int CityId { get; set; }

        public string CityIataCode { get; set; }

        public string CityName { get; set; }
    }
}
