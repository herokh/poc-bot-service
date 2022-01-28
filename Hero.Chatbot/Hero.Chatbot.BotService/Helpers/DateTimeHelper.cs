using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Hero.Chatbot.BotService.Helpers
{
    public static class DateTimeHelper
    {
        public static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
