using Hero.Chatbot.ApiGateway.Models;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.HttpClients
{
    public interface ICognitiveClient
    {
        Task<CognitiveToken> GenerateToken();
    }
}
