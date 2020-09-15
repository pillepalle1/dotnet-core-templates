using Pillepalle1.ConsoleTelegramBot.Model.Misc;
using Telegram.Bot.Types;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    public class MessageHandlerArgs
    {
        public MessageHandlerArgs(Message message)
        {
            _message = message;
        }

        /// <summary>
        /// Message object the bot has received
        /// </summary>
        public Message Message
        {
            get
            {
                return _message;
            }
        }
        private Message _message = null;

        /// <summary>
        /// If the message contains a text, it can be accessed token-wise
        /// </summary>
        public CommandTokenizer CommandTokens
        {
            get
            {
                if (null == _commandTokenizer)
                {
                    if (!string.IsNullOrWhiteSpace(Message.Text))
                    {
                        _commandTokenizer = new CommandTokenizer(Message.Text);
                    }
                }

                return _commandTokenizer;
            }
        }
        private CommandTokenizer _commandTokenizer = null;
    }
}
