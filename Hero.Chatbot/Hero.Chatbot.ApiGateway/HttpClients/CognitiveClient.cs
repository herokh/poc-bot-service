using Hero.Chatbot.ApiGateway.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.HttpClients
{
    public class CognitiveClient : ICognitiveClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public CognitiveClient(IConfiguration configuration,
            HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://southeastasia.api.cognitive.microsoft.com");
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<CognitiveToken> GenerateToken()
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.GetValue<string>("Cognitive:SubscriptionKey"));
            var response = await _httpClient.PostAsync("/sts/v1.0/issuetoken", null);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return new CognitiveToken { Token = result };
        }
    }
}
