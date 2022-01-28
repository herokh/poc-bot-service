using Hero.Chatbot.BotService.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FlightBookingRecognizerResult = Hero.Chatbot.BotService.CognitiveModels.FlightBooking;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class MainDialog : DialogBase
    {
        private const string GreetingMsgText = "Hi I'm HeroBot, your assistance. How can I help you today?";
        private const string SelectOptionMsgText = "Please select the following Option";
        private const string DidntUnderstandMsgText = "Sorry I didn't understand. Please try asking in different way.";
        private const string endConversationMsgText = "Is there anything I can help you?";
        private const string FallbackMsgText = "Please tell your question again.";

        private readonly string _greetingCard = Path.Combine(".", "Resources", "GreetingCard.json");

        protected readonly ILogger Logger;
        private readonly FlightBookingRecognizer _luisRecognizer;

        public MainDialog(ILogger<MainDialog> logger,
            FlightSearchDialog flightInfoDialog,
            PromotionDialog promotionDialog,
            MoreOptionDialog moreOptionDialog,
            FlightManagementDialog flightManagementDialog,
            GeneralInformationDialog generalInformationDialog,
            FlightBookingRecognizer luisRecognizer)
        : base(nameof(MainDialog))
        {
            Logger = logger;
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(flightInfoDialog);
            AddDialog(promotionDialog);
            AddDialog(moreOptionDialog);
            AddDialog(flightManagementDialog);
            AddDialog(generalInformationDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                LuisRecognizerStepAsync,
                //ActStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);

        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var greetingText = stepContext.Options?.ToString() ?? GreetingMsgText;
            var greetingMessage = MessageFactory.Text(greetingText, greetingText, InputHints.IgnoringInput);
            //await stepContext.Context.SendActivityAsync(greetingMessage, cancellationToken);

            //var selectOptionAttachment = AdaptiveCardHelper.CreateAdaptiveCardAttachment(_greetingCard);
            //var selectOptionMessage = MessageFactory.Attachment(selectOptionAttachment, SelectOptionMsgText, SelectOptionMsgText, InputHints.ExpectingInput);
            //await stepContext.Context.SendActivityAsync(selectOptionMessage, cancellationToken);

            //return await stepContext.BeginDialogAsync(nameof(FlightSearchDialog), new FlightSearchModel(), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = greetingMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> LuisRecognizerStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (_luisRecognizer.IsConfigured)
            {
                var recognizerResult = await _luisRecognizer.RecognizeAsync<FlightBookingRecognizerResult>(stepContext.Context, cancellationToken);

                switch (recognizerResult.TopIntent().intent)
                {
                    case FlightBookingRecognizerResult.Intent.Check_availability:
                        var flightSearchModel = new FlightSearchModel
                        {
                            Origin = recognizerResult.Origin,
                            Destination = recognizerResult.Destination,
                            DepartureDate = recognizerResult.DateTime
                        };
                        return await stepContext.BeginDialogAsync(nameof(FlightSearchDialog), flightSearchModel, cancellationToken);
                    case FlightBookingRecognizerResult.Intent.Manage_booking:
                        var managementModel = new FlightManagementModel
                        {
                            OrderDate = recognizerResult.DateTime
                        };
                        return await stepContext.BeginDialogAsync(nameof(FlightManagementDialog), managementModel, cancellationToken);
                    case FlightBookingRecognizerResult.Intent.Check_information:
                        var generalModel = new GeneralInformationModel
                        {
                            ProductType = recognizerResult.ProductType
                        };
                        return await stepContext.BeginDialogAsync(nameof(GeneralInformationDialog), generalModel, cancellationToken);
                    default:
                        var fallbackMessageText = MessageFactory.Text(FallbackMsgText, FallbackMsgText, InputHints.ExpectingInput);
                        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = fallbackMessageText }, cancellationToken);
                }
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var answer = ((string)stepContext.Result).ToLower().Replace(".", "");

            switch (answer)
            {
                case "check promotion":
                    return await stepContext.BeginDialogAsync(nameof(PromotionDialog), new PromotionModel(), cancellationToken);

                case "check flight":
                    return await stepContext.BeginDialogAsync(nameof(FlightSearchDialog), new FlightSearchModel(), cancellationToken);

                case "more option":
                    return await stepContext.BeginDialogAsync(nameof(MoreOptionDialog), null, cancellationToken);

                default:
                    var didntUnderstandMessage = MessageFactory.Text(DidntUnderstandMsgText, DidntUnderstandMsgText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is FlightBookingModel result)
            {
                var travelDateTimex = new TimexProperty(result.TravelDate);
                var travelDateMsg = travelDateTimex.ToNaturalLanguage(DateTime.Now);
                var messageText = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            return await stepContext.ReplaceDialogAsync(InitialDialogId, endConversationMsgText, cancellationToken);
        }

    }
}
