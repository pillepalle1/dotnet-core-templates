using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Pillepalle1.ConsoleTelegramBot.Model.Misc;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    public class UpdateHandlerArgs
    {
        public UpdateHandlerArgs(ITelegramBotClient botClient, Update update)
        {
            _botClient = botClient;
            _update = update;
        }
  
        /// <summary>
        /// Flag indicating whether the update has been successfully processed
        /// </summary>
        public bool Handled
        {
            get
            {
                return _isHandled;
            }
            set
            {
                if (value)
                {
                    _isHandled = true;
                }
            }
        }
        private bool _isHandled = false;

        /// <summary>
        /// Reference to the Bot-Object for sending messages
        /// </summary>
        public ITelegramBotClient BotClient
        {
            get
            {
                return _botClient;
            }
        }
        private ITelegramBotClient _botClient = null;

        /// <summary>
        /// Object of the Update that needs to be handled
        /// </summary>
        public Update Update
        {
            get
            {
                return _update;
            }
        }
        private Update _update = null;

        /// <summary>
        /// If the Update is a Message, it can easily be accessed here
        /// </summary>
        public MessageArgs MessageArgs
        {
            get
            {
                if (null == _messageHandlerArgs)
                {
                    if (Update.Type == UpdateType.Message)
                    {
                        _messageHandlerArgs = new MessageArgs(Update.Message);
                    }
                }

                return _messageHandlerArgs;
            }
        }
        private MessageArgs _messageHandlerArgs = null;
    }
}