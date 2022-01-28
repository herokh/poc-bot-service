using Hero.Chatbot.BotService.Models;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class GeneralInformationDialog : CancelAndHelpDialog
    {
        protected readonly ILogger Logger;
        private readonly IFlightService _flightService;
        private readonly FlightBookingRecognizer _luisRecognizer;

        public GeneralInformationDialog(ILogger<GeneralInformationDialog> logger,
            IFlightService flightService,
            FlightBookingRecognizer luisRecognizer) : base(nameof(GeneralInformationDialog))
        {
            Logger = logger;
            _flightService = flightService;
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]{
                InitialStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (GeneralInformationModel)stepContext.Options;
            var messageString = $"Go to {model.ProductType} information page.";
            var messageActivity = MessageFactory.Text(messageString, messageString, messageString);
            await stepContext.Context.SendActivityAsync(messageActivity, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = messageActivity }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
