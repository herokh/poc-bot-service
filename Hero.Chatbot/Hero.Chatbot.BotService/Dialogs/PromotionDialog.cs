using Hero.Chatbot.BotService.Helpers;
using Hero.Chatbot.Service.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class PromotionDialog : CancelAndHelpDialog
    {
        private const string PromotionMsgText = "Found {0} promotions!";
        private const string NotFoundPromotionMsgText = "No promotions right now.";
        private readonly string _promotionsCard = Path.Combine(".", "Resources", "PromotionsCard.json");

        protected readonly ILogger Logger;
        private readonly IPromotionService _promotionService;

        public PromotionDialog(IPromotionService promotionService,
            ILogger<PromotionDialog> logger)
            : base(nameof(PromotionDialog))
        {
            Logger = logger;
            _promotionService = promotionService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                DisplayPromotionStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayPromotionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promotions = _promotionService.GetPromotions();

            if (promotions.Any())
            {
                var promotionCards = promotions.Select(p => AdaptiveCardHelper.CreateAdaptiveCardAttachment(_promotionsCard, p));
                var promotionMsgText = string.Format(PromotionMsgText, promotionCards.Count());
                var promotionMessage = MessageFactory.Carousel(promotionCards, promotionMsgText, promotionMsgText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(promotionMessage, cancellationToken);
            }
            else
            {
                var notFoundPromotionMessage = MessageFactory.Text(NotFoundPromotionMsgText, NotFoundPromotionMsgText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(notFoundPromotionMessage, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
