using Hero.Chatbot.BotService.Helpers;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class MoreOptionDialog : CancelAndHelpDialog
    {
        private const string SelectServicesMsgText = "Please select the following Option";

        private readonly string _moreOptionCard = Path.Combine(".", "Resources", "MoreOptionCard.json");

        protected readonly ILogger Logger;

        public MoreOptionDialog(ILogger<MoreOptionDialog> logger,
            IExtraServiceService extraServiceService)
            : base(nameof(MoreOptionDialog))
        {
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ExtraServicesDialog(extraServiceService));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                SelectServicesStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> SelectServicesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var moreOptionCard = AdaptiveCardHelper.CreateAdaptiveCardAttachment(_moreOptionCard);
            var moreOptionAttachment = MessageFactory.Attachment(moreOptionCard, SelectServicesMsgText, SelectServicesMsgText, InputHints.ExpectingInput);
            await stepContext.Context.SendActivityAsync(moreOptionAttachment, cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions(), cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = ((string)stepContext.Result).ToLower();
            switch (msg)
            {
                case "extra service":
                    await stepContext.BeginDialogAsync(nameof(ExtraServicesDialog), null, cancellationToken);
                    break;
                default:
                    var message = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(message, cancellationToken);
                    break;
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
