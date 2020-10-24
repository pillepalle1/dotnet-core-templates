using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    public abstract class ACommandTextMessageUpdateHandler : AMessageUpdateHandler
    {
        /// <summary>
        /// Commands are only accepted from Text-Messages
        /// </summary>
        protected override MessageType? HandlesMessageType => MessageType.Text;

        /// <summary>
        /// Filters for supported commands
        /// </summary>
        protected override async Task MessageUpdateHandlerLogicImpl(UpdateHandlerArgs args)
        {
            if (!HandlesCommands.Contains(args.MessageArgs.CommandTokens[0].Trim().ToLower()))
            {
                return;
            }

            await CommandTextMessageUpdateHandlerLogicImpl(args);
        }

        /// <summary>
        /// Implements the logic for the selected commands
        /// </summary>
        protected abstract Task CommandTextMessageUpdateHandlerLogicImpl(UpdateHandlerArgs args);

        /// <summary>
        /// Basic filter logic for pre-selected commands
        /// </summary>
        public abstract IList<string> HandlesCommands { get; }
    }
}
