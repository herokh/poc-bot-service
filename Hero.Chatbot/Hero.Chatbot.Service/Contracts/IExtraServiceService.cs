using Hero.Chatbot.Domain.ExtraService;
using System.Collections.Generic;

namespace Hero.Chatbot.Service.Contracts
{
    public interface IExtraServiceService
    {
        IEnumerable<ExtraService> GetExtraServices();
    }
}
