using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService
{
    public class FlightBookingRecognizer : IRecognizer
    {
        private readonly LuisRecognizer _luisRecognizer;

        public FlightBookingRecognizer(IConfiguration configuration)
        {
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) && !string.IsNullOrEmpty(configuration["LuisAPIKey"]) && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);

            if (luisIsConfigured)
            {
                var application = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://" + configuration["LuisAPIHostName"]);
                var options = new LuisRecognizerOptionsV3(application)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                        Slot = "Staging"
                    }
                };

                _luisRecognizer = new LuisRecognizer(options);
            }

        }

        public virtual bool IsConfigured => _luisRecognizer != null;

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
        }

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
        {
            return await _luisRecognizer.RecognizeAsync<T>(turnContext, cancellationToken);
        }
    }
}
