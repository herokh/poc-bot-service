using System.Linq;

namespace Hero.Chatbot.BotService.CognitiveModels
{
    public partial class FlightBooking
    {
        public string Origin
        {
            get
            {
                var origin = Entities?._instance?.Origin?.FirstOrDefault()?.Text;
                return origin;
            }
        }

        public string Destination
        {
            get
            {
                var destination = Entities?._instance?.Destination?.FirstOrDefault()?.Text;
                return destination;
            }
        }

        public string DateTime
        {
            get
            {
                var departureDate = Entities?.datetime?.FirstOrDefault()?.Expressions?.FirstOrDefault()?.Split('T')[0];
                return departureDate;
            }
        }

        public string ProductType
        {
            get
            {
                var productType = Entities?._instance?.ProductType?.FirstOrDefault()?.Text;
                return productType;
            }
        }
    }
}
