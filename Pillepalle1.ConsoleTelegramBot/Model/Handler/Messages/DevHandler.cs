using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler.Messages
{
    public class DevHandler : AUpdateHandler<UpdateHandlerArgs>
    {
        protected override UpdateType? HandlesUpdateType => UpdateType.Message;

        protected override async Task HandlerLogicImpl(UpdateHandlerArgs args)
        {
            StringBuilder b = new StringBuilder();

            b.AppendLine("<code>Parsed Tokens</code>");
            for (int i = 0; i < args.MessageHandlerArgs.CommandTokens.Count; i++)
            {
                b.AppendLine($"<code>[{i}] </code>{args.MessageHandlerArgs.CommandTokens[i]}");
            }

            await args.BotClient.SendTextMessageAsync(
                args.MessageHandlerArgs.Message.Chat.Id, 
                b.ToString(),
                parseMode: ParseMode.Html);
        }
    }
}
