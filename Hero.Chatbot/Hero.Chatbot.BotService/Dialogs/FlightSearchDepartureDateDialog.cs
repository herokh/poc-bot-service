using Hero.Chatbot.BotService.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class FlightSearchDepartureDateDialog : CancelAndHelpDialog
    {
        private const string SelectDepartureDateMsgText = "Select the departure date";
        private const string DepartureDateInvalidMsgText = "Departure date is invalid. please enter a full departure date including Day Month and Year.";

        public FlightSearchDepartureDateDialog(string id = null)
            : base(id ?? nameof(FlightSearchDepartureDateDialog))
        {

            AddDialog(new DateTimePrompt(nameof(DateTimePrompt), DateTimePromptValidator));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DepartureDateStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DepartureDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var departureDate = (string)stepContext.Options;

            if (departureDate == null)
            {
                var promptMessage = MessageFactory.Text(SelectDepartureDateMsgText, SelectDepartureDateMsgText, InputHints.ExpectingInput);
                var repromptMessage = MessageFactory.Text(DepartureDateInvalidMsgText, DepartureDateInvalidMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(DateTimePrompt), new PromptOptions { Prompt = promptMessage, RetryPrompt = repromptMessage }, cancellationToken);
            }

            if (DateTimeHelper.IsAmbiguous(departureDate))
            {
                var promptMessage = MessageFactory.Text(DepartureDateInvalidMsgText, DepartureDateInvalidMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(DateTimePrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(new DateTimeResolution { Timex = departureDate }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var timex = ((IList<DateTimeResolution>)stepContext.Result)[0].Timex;
            return await stepContext.EndDialogAsync(timex, cancellationToken);
        }

        private Task<bool> DateTimePromptValidator(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var timex = promptContext.Recognized.Value[0].Timex.Split('T')[0];
                var isDefinite = new TimexProperty(timex).Types.Contains(Constants.TimexTypes.Definite);
                return Task.FromResult(isDefinite);
            }
            return Task.FromResult(false);
        }

    }
}
