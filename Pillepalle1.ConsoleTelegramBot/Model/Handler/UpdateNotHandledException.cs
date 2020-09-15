using System;
using System.Runtime.Serialization;
using Telegram.Bot.Types;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    [Serializable]
    internal class UpdateNotHandledException : Exception
    {
        public Update Update
        {
            get
            {
                return _update;
            }
        }
        private Update _update;

        public UpdateNotHandledException()
        {
        }

        public UpdateNotHandledException(Update update)
        {
            this._update = update;
        }

        public UpdateNotHandledException(string message) : base(message)
        {
        }

        public UpdateNotHandledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UpdateNotHandledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}