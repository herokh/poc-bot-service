using Hero.Chatbot.BotService.Helpers;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class ExtraServicesDialog : CancelAndHelpDialog
    {
        private readonly string _extraServicesCard = Path.Combine(".", "Resources", "ExtraServicesCard.json");

        private readonly IExtraServiceService _extraServiceService;

        public string SelectExtraServicesMsgText { get; private set; }

        public ExtraServicesDialog(IExtraServiceService extraServiceService)
            : base(nameof(ExtraServicesDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                DisplayExtraServicesStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _extraServiceService = extraServiceService;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayExtraServicesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var extraServices = _extraServiceService.GetExtraServices();
            var card = AdaptiveCardHelper.CreateAdaptiveCardAttachment(_extraServicesCard, new { Items = extraServices });
            var extraServicesMessage = MessageFactory.Attachment(card, SelectExtraServicesMsgText, SelectExtraServicesMsgText, InputHints.ExpectingInput);
            await stepContext.Context.SendActivityAsync(extraServicesMessage, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = (string)stepContext.Result;
            var selectedExtraService = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(selectedExtraService, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
