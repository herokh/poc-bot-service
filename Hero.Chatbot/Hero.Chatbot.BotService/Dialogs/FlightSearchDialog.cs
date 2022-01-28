using Hero.Chatbot.BotService.Helpers;
using Hero.Chatbot.BotService.Models;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightBookingModel = Hero.Chatbot.BotService.CognitiveModels.FlightBooking;


namespace Hero.Chatbot.BotService.Dialogs
{
    public class FlightSearchDialog : CancelAndHelpDialog
    {
        private const string SuggestionMsgText = "Ask something like \"I want to check flight from Bangkok to Chiang mai on tomorrow.\"";
        private const string OriginMsgText = "Please input your origin.";
        private const string DestinationMsgText = "Please input your destination.";

        private const string FlightNotFouldMsgText = "Flight not found from {0} to {1} on {2}";

        private readonly string _flightCard = Path.Combine(".", "Resources", "FlightReservationCard.json");

        protected readonly ILogger Logger;
        private readonly IFlightService _flightService;
        private readonly FlightBookingRecognizer _luisRecognizer;
        private readonly IStorage _storage;


        public FlightSearchDialog(ILogger<FlightSearchDialog> logger,
            IFlightService flightService,
            FlightBookingRecognizer luisRecognizer,
            IStorage storage)
            : base(nameof(FlightSearchDialog))
        {
            Logger = logger;
            _flightService = flightService;
            _luisRecognizer = luisRecognizer;
            _storage = storage;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new FlightSearchDepartureDateDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]{
                InitialStepAsync,
                LuisRecognizerStepAsync,
                OriginStepAsync,
                DestinationStepAsync,
                DepartureDateStepAsync,
                DisplayFlightsStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text(SuggestionMsgText, SuggestionMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> LuisRecognizerStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (_luisRecognizer.IsConfigured)
            {
                var recognizerResult = await _luisRecognizer.RecognizeAsync<FlightBookingModel>(stepContext.Context, cancellationToken);

                switch (recognizerResult.TopIntent().intent)
                {
                    case FlightBookingModel.Intent.Check_availability:
                        var model = (FlightSearchModel)stepContext.Options;
                        model.Origin = recognizerResult.Origin;
                        model.Destination = recognizerResult.Destination;
                        model.DepartureDate = recognizerResult.DateTime;

                        return await stepContext.NextAsync(null, cancellationToken);
                    default:
                        var promptMessage = MessageFactory.Text(SuggestionMsgText, SuggestionMsgText, InputHints.ExpectingInput);
                        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
                }
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (FlightSearchModel)stepContext.Options;

            if (string.IsNullOrEmpty(model.Origin))
            {
                var promptMessage = MessageFactory.Text(OriginMsgText, OriginMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(model.Origin, cancellationToken);
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (FlightSearchModel)stepContext.Options;
            model.Origin = (string)stepContext.Result;

            if (string.IsNullOrEmpty(model.Destination))
            {
                var promptMessage = MessageFactory.Text(DestinationMsgText, DestinationMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(model.Destination, cancellationToken);
        }

        private async Task<DialogTurnResult> DepartureDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (FlightSearchModel)stepContext.Options;
            model.Destination = (string)stepContext.Result;

            if (string.IsNullOrEmpty(model.DepartureDate) || DateTimeHelper.IsAmbiguous(model.DepartureDate))
            {
                return await stepContext.BeginDialogAsync(nameof(FlightSearchDepartureDateDialog), model.DepartureDate, cancellationToken);
            }

            return await stepContext.NextAsync(model.DepartureDate, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayFlightsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var model = (FlightSearchModel)stepContext.Options;
            model.DepartureDate = (string)stepContext.Result;

            var flights = _flightService.GetFlights(model.ToFlightFilter());

            if (flights.Any())
            {
                //var flightMessages = MessageFactory.Carousel(flights.Select(f => {
                //    return AdaptiveCardHelper.CreateAdaptiveCardAttachment(_flightCard, f);
                //}));

                //await stepContext.Context.SendActivitiesAsync(new Activity[] {
                //    (Activity)flightMessages
                //}, cancellationToken);

                var foundFlightMsgText = $"Found {flights.Count()} flights. the system will redirect for you!";
                var foundFlightMessage = MessageFactory.Text(foundFlightMsgText, foundFlightMsgText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(foundFlightMessage, cancellationToken);

                var flightResultReply = stepContext.Context.Activity.CreateReply();
                flightResultReply.Type = ActivityTypes.Event;
                flightResultReply.Name = "searchFlight";
                flightResultReply.Value = model.ToFlightFilter();

                await stepContext.Context.SendActivityAsync(flightResultReply, cancellationToken);

                var changes = new Dictionary<string, object>
                {
                    { $"searchFlight_{stepContext.Context.Activity.Conversation.Id}", JObject.FromObject(flightResultReply) }
                };

                await _storage.WriteAsync(changes, cancellationToken);
            }
            else
            {
                var flightNotFoundText = string.Format(FlightNotFouldMsgText, model.Origin, model.Destination, DateTime.Parse(model.DepartureDate).ToString("dddd, dd MMMM yyyy"));
                var flightNotFoundMessage = MessageFactory.Text(flightNotFoundText, flightNotFoundText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(flightNotFoundMessage, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
