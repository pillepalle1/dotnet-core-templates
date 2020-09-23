using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler.Messages
{
    public class DevHandler : AUpdateHandler
    {
        protected override UpdateType? HandlesUpdateType => UpdateType.Message;

        protected override async Task HandlerLogicImpl(UpdateHandlerArgs args)
        {
            StringBuilder b = new StringBuilder();

            b.AppendLine("<code>Parsed Tokens</code>");
            for (int i = 0; i < args.MessageArgs.CommandTokens.Count; i++)
            {
                b.AppendLine($"<code>[{i}] </code>{args.MessageArgs.CommandTokens[i]}");
            }

            await args.BotClient.SendTextMessageAsync(
                args.MessageArgs.Message.Chat.Id, 
                b.ToString(),
                parseMode: ParseMode.Html);
        }
    }
}
