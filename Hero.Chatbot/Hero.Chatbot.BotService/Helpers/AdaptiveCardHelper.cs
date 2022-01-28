using AdaptiveCards.Templating;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.IO;

namespace Hero.Chatbot.BotService.Helpers
{
    public static class AdaptiveCardHelper
    {
        public static Attachment CreateAdaptiveCardAttachment(string filePath, object data = null)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var template = new AdaptiveCardTemplate(adaptiveCardJson);
            var context = new EvaluationContext(data);
            var cardJson = template.Expand(context);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };
            return adaptiveCardAttachment;
        }
    }
}
