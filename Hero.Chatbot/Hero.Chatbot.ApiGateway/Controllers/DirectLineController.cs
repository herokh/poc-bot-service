using Hero.Chatbot.ApiGateway.HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hero.Chatbot.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectLineController : ControllerBase
    {
        private readonly IDirectLineClient _directLineClient;
        public DirectLineController(IDirectLineClient directLineClient)
        {
            _directLineClient = directLineClient;
        }

        [HttpPost("generateToken")]
        [ProducesResponseType(200, Type = typeof(Models.DirectLineToken))]
        public async Task<ActionResult> GenerateTokenAsync()
        {
            var result = await _directLineClient.GenerateToken();
            return Ok(result);
        }
    }
}
