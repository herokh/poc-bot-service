using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Dialogs
{
    public class DialogBase : ComponentDialog
    {
        public DialogBase(string dialogId) :
            base(dialogId)
        {
        }


        public override async Task<bool> OnDialogEventAsync(DialogContext dc, DialogEvent e, CancellationToken cancellationToken)
        {
            return await base.OnDialogEventAsync(dc, e, cancellationToken);
        }
    }
}
