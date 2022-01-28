using Hero.Chatbot.Domain.Promotion;
using System.Collections.Generic;

namespace Hero.Chatbot.Service.Contracts
{
    public interface IPromotionService
    {
        IEnumerable<Promotion> GetPromotions();
    }
}
