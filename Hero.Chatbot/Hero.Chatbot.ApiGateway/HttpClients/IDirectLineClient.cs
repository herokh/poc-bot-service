using Hero.Chatbot.ApiGateway.Models;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.HttpClients
{
    public interface IDirectLineClient
    {
        Task<DirectLineToken> GenerateToken();
        Task RefreshToken(string token);
    }
}
