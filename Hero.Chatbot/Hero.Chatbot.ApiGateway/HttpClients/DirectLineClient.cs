using Hero.Chatbot.ApiGateway.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.HttpClients
{
    public class DirectLineClient : IDirectLineClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DirectLineClient(HttpClient httpClient,
            IConfiguration configuration)
        {
            httpClient.BaseAddress = new Uri("https://directline.botframework.com");
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<DirectLineToken> GenerateToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                _configuration.GetValue<string>("DirectLine:Secretkey"));
            var response = await _httpClient.PostAsync("/v3/directline/tokens/generate", null);
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DirectLineToken>(resultString);

            return result;
        }

        public async Task RefreshToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                token);
            var response = await _httpClient.PostAsync("/v3/directline/tokens/refresh", null);

            response.EnsureSuccessStatusCode();
        }
    }
}
