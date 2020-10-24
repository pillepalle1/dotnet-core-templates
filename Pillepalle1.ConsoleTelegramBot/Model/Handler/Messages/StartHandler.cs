using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler.Messages
{
    public class StartHandler : AUpdateHandler
    {
        protected override UpdateType? HandlesUpdateType => UpdateType.Message;

        protected override async Task HandlerLogicImpl(UpdateHandlerArgs args)
        {
            // Thanks to HandlesUpdateType we already know the Update is of type Message
            Message m = args.MessageArgs.Message;

            // We only process text-messages
            if (m.Type == MessageType.Text)
            {
                if (args.MessageArgs.CommandTokens[0].ToLower().Equals("/start"))
                {
                    await args.BotClient.SendTextMessageAsync(
                        m.Chat.Id,
                        "Hello Telegram World"
                    );

                    // Make sure to mark Update as handled to hide it from successive handlers
                    args.Handled = true;
                }
            }
        }
    }
}