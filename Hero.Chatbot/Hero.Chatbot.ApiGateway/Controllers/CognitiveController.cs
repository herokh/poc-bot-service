using Hero.Chatbot.ApiGateway.HttpClients;
using Hero.Chatbot.ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CognitiveController : ControllerBase
    {
        private readonly ICognitiveClient _cognitiveClient;
        public CognitiveController(ICognitiveClient cognitiveClient)
        {
            _cognitiveClient = cognitiveClient;
        }

        [HttpPost("generateToken")]
        [ProducesResponseType(200, Type = typeof(CognitiveToken))]
        public async Task<ActionResult> GenerateTokenAsync()
        {
            var result = await _cognitiveClient.GenerateToken();
            return Ok(result);
        }
    }
}
