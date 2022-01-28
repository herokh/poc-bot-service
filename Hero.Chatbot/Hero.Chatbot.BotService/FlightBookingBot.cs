// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService
{
    public class FlightBookingBot<T> : ActivityHandler
        where T : Dialog
    {
        protected readonly T Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;
        protected readonly IStorage Storage;

        public FlightBookingBot(T dialog,
            ConversationState conversationState,
            UserState userState,
            ILogger<FlightBookingBot<T>> logger, IStorage storage)
        {
            Dialog = dialog;
            ConversationState = conversationState;
            UserState = userState;
            Logger = logger;
            Storage = storage;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeMessage = "Welcome to Hero Happy Travel Company";
                    var message = MessageFactory.Text(welcomeMessage, welcomeMessage, InputHints.IgnoringInput);
                    await turnContext.SendActivityAsync(message, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            base.OnEventActivityAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == "event" && turnContext.Activity.Name == "EVENT_DELETE_SEARCH_FLIGHT")
            {
                var searchFlightKey = $"searchFlight_{turnContext.Activity.Conversation.Id}";
                var changes = await Storage.ReadAsync(new[] { searchFlightKey }, cancellationToken);
                if (changes.TryGetValue(searchFlightKey, out var v))
                {
                    var searchFlightEvent = ((JObject)v).ToObject<Activity>();
                    searchFlightEvent.Name = $"{searchFlightEvent.Name}_Done";
                    await turnContext.UpdateActivityAsync(searchFlightEvent, cancellationToken);
                }
            }
        }
    }
}
