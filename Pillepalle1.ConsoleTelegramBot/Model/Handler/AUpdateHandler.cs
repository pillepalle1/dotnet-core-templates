using System;
using System.Threading.Tasks;

using Telegram.Bot.Types.Enums;

using Pillepalle1.ConsoleTelegramBot.Model.Misc;

namespace Pillepalle1.ConsoleTelegramBot.Model.Handler
{
    public abstract class AUpdateHandler
    {

        public AUpdateHandler(AUpdateHandler next = null)
        {
            _nextHandler = next;
        }

        /// <summary>
        /// Passes an update to the chain for handling
        /// </summary>
        /// <param name="args">Data structure containing relevant references for update handling</param>
        public async Task Handle(UpdateHandlerArgs args)
        {
            // Catch exceptions so they don't cause the entire service to crash
            try
            {
                if ((null == HandlesUpdateType) || (args.Update.Type == HandlesUpdateType))
                {
                    await HandlerLogicImpl(args);
                }
            }
            catch (Exception e)
            {
                await Say.Error($"Internal error while handling update {args.Update.Id}: {e.Message}");
            }

            // End of the chain has reached as soon as the update has been marked as handled
            if (args.Handled)
            {
                return;
            }

            // Invoke next handler in chain, otherwise throw Exception that update was not handled
            if (null != _nextHandler)
            {
                await _nextHandler.Handle(args);
            }
            else
            {
                throw new UpdateNotHandledException(args.Update);
            }
        }

        /// <summary>
        /// Implements the actual logic for handling upats
        /// </summary>
        /// <param name="args">Data structure containing relevant references for update handling</param>
        protected abstract Task HandlerLogicImpl(UpdateHandlerArgs args);

        /// <summary>
        /// Registers a handler at the end of the chain
        /// </summary>
        /// <param name="handler">Handler to be registered</param>
        public void Add(AUpdateHandler handler)
        {
            if (null == _nextHandler)
            {
                _nextHandler = handler;
            }
            else
            {
                _nextHandler.Add(handler);
            }
        }

        /// <summary>
        /// Points to the next handler in the chain
        /// </summary>
        public AUpdateHandler NextHandler
        {
            get
            {
                return _nextHandler;
            }
        }
        private AUpdateHandler _nextHandler = null;

        /// <summary>
        /// Defines what type of Updates can be handled here. null for all
        /// </summary>
        protected abstract UpdateType? HandlesUpdateType { get; }
    }
}
