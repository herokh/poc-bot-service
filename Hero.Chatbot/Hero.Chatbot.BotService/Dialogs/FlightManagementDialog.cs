using Hero.Chatbot.BotService.Models;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class FlightManagementDialog : CancelAndHelpDialog
    {
        protected readonly ILogger Logger;
        private readonly IFlightService _flightService;
        private readonly FlightBookingRecognizer _luisRecognizer;

        public FlightManagementDialog(ILogger<FlightManagementDialog> logger,
            IFlightService flightService,
            FlightBookingRecognizer luisRecognizer) : base(nameof(FlightManagementDialog))
        {
            Logger = logger;
            _flightService = flightService;
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]{
                InitialStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (FlightManagementModel)stepContext.Options;

            if (string.IsNullOrEmpty(model.OrderDate))
            {
                var askingReservationMessage = "Do you have a reservation?";
                var askingReservationActivity = MessageFactory.Text(askingReservationMessage, askingReservationMessage, askingReservationMessage);
                //await stepContext.Context.SendActivityAsync(messageActivity, cancellationToken);
                //return await stepContext.NextAsync(null, cancellationToken);

                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = askingReservationActivity }, cancellationToken);
            }

            return await stepContext.NextAsync(true, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var isConfirmed = (bool)stepContext.Result;

            if (isConfirmed)
            {
                var msg = "Go to Manage booking page";
                var msgActivity = MessageFactory.Text(msg, msg, msg);
                await stepContext.Context.SendActivityAsync(msgActivity, cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(FlightSearchDialog), new FlightSearchModel(), cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
