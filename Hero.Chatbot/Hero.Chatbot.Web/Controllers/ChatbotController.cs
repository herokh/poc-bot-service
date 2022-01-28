using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hero.Chatbot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private const string ConversationToken = "conversation_token";
        private const string SpeechToken = "speech_token";

        private readonly ISession _session;
        private readonly IHeroChatbotCMSApiClient _heroChatbotCMSApiClient;
        public ChatbotController(IHeroChatbotCMSApiClient heroChatbotCMSApiClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _heroChatbotCMSApiClient = heroChatbotCMSApiClient;
            _session = httpContextAccessor.HttpContext.Session;
        }

        [HttpPost("Token")]
        public async Task<IActionResult> GenerateConversationToken()
        {
            var conversationToken = _session.GetString(ConversationToken);
            if (conversationToken == null)
            {
                conversationToken = (await _heroChatbotCMSApiClient.GenerateToken2Async()).Token;
                _session.SetString(ConversationToken, conversationToken);
            }

            var speechToken = _session.GetString(SpeechToken);
            if (speechToken == null)
            {
                speechToken = (await _heroChatbotCMSApiClient.GenerateTokenAsync()).Token;
                _session.SetString(SpeechToken, speechToken);
            }

            return Ok(new { c = conversationToken, s = speechToken });
        }
    }
}
