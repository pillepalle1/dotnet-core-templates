using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    public abstract class AMessageUpdateHandler : AUpdateHandler
    {
        /// <summary>
        /// Only Accept Updates of Type Message
        /// </summary>
        protected override UpdateType? HandlesUpdateType => UpdateType.Message;

        /// <summary>
        /// Basic filtering for messages of type MessageUpdateType
        /// </summary>
        protected override async Task HandlerLogicImpl(UpdateHandlerArgs args)
        {
            if (null != HandlesMessageType)
            {
                if(args.MessageArgs.Message.Type != HandlesMessageType)
                {
                    return;
                }
            }

            await MessageUpdateHandlerLogicImpl(args);
        }

        /// <summary>
        /// Logic for handling messages of type HandlesMessageType
        /// </summary>
        protected abstract Task MessageUpdateHandlerLogicImpl(UpdateHandlerArgs args);

        /// <summary>
        /// Setting this Property allows to filter out unsupported MessageTypes
        /// </summary>
        protected abstract MessageType? HandlesMessageType { get; }
    }
}
