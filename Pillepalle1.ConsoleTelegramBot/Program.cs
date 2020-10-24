using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

using Pillepalle1.ConsoleTelegramBot.Model.Misc;
using Pillepalle1.ConsoleTelegramBot.Model.Handler;
using Pillepalle1.ConsoleTelegramBot.Model.Handler.Messages;

namespace Pillepalle1.ConsoleTelegramBot
{
    class Program
    {
        private static ManualResetEvent _TerminateProcessEvent = new ManualResetEvent(false);
        private static CancellationTokenSource _CancelWorkerTasks = new CancellationTokenSource();

        private static ITelegramBotClient _Bot = null;
        private static Channel<Update> _BotUpdatesChannel = null;
        private static AUpdateHandler _UpdateHandlerChain = null;

        static async Task Main(string[] args)
        {
            // ------------------------------------------------------------------------------------
            // Step 1: Verify and/ or set up environement
            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BOT_TOKEN")))
            {
                // Environment.SetEnvironmentVariable("BOT_TOKEN", "");
                await Say.Error("Fatal: Environment Variable BOT_TOKEN empty");
                return;
            }

            // ------------------------------------------------------------------------------------
            // Step 2 - Setting up the bot object
            _UpdateHandlerChain = new DevHandler();
            _UpdateHandlerChain.Add(new StartHandler());

            _Bot = new TelegramBotClient(Environment.GetEnvironmentVariable("BOT_TOKEN"));

            _BotUpdatesChannel = Channel.CreateUnbounded<Update>(
            new UnboundedChannelOptions()
            {
                SingleWriter = true,
                SingleReader = true
            });

            var updateHandler = Task.Run(async () => await _HandleUpdates());
            var updateFetcher = Task.Run(async () => await _FetchUpdates(_CancelWorkerTasks.Token));

            // Setting up Event Handler for Smooth Termination ( Improved Thread.Sleep(forever) )
            Console.CancelKeyPress += async (object sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;                                        // Intercept the CTRL+C
                _TerminateProcessEvent.Set();                           // Signal the main thread

                await Say.Warning("Cancellation request sent");
            };

            _TerminateProcessEvent.WaitOne();                           // Suspend main thread until signaled

            // ------------------------------------------------------------------------------------
            // Step 3 - Tidy up: Release ressources, flush buffers, etc
            _CancelWorkerTasks.Cancel();
            await Task.WhenAll(updateFetcher, updateHandler);

            await Say.Success("Thank you very much. You made a simple bot very happy");
        }

        /// <summary>
        /// Spawns a Task (Producer) that is responsible for periodically fetching updates from the Telegram network
        /// </summary>
        /// <param name="cancellationToken">Used for smooth termination</param>
        private static async Task _FetchUpdates(CancellationToken cancellationToken)
        {
            await Say.Verbose("Task for fetching updates started");

            var offset = -1;
            var delay = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(delay, cancellationToken);

                        var updates = await _Bot.GetUpdatesAsync(offset: offset, cancellationToken: cancellationToken);
                        offset = updates.Length > 0 ? updates[^1].Id + 1 : -1;

                        foreach (var update in updates)
                        {
                            await _BotUpdatesChannel.Writer.WriteAsync(update);
                        }

                        delay = 0;
                    }
                    catch (ApiRequestException apiRequestException)
                    {
                        await Say.Warning($"Prevented crash caused by ApiRequestException");
                        await Say.Warning($"> {apiRequestException.Message}");

                        delay = Math.Min(
                            Math.Max(1000, 2 * delay),
                            1000 * 60 * 5);
                    }
                    catch (HttpRequestException httpRequestException)
                    {
                        await Say.Warning($"Prevented crash caused by HttpRequestException");
                        await Say.Warning($"> {httpRequestException.Message}");
                    }
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                await Say.Verbose("Task for fetching updates received signal to shut down");
            }
            catch(Exception generalException)
            {
                await Say.Error("Task for fetching updates crashed unexpectedly");
                await Say.Error($"> [{generalException.GetType()}] {generalException.Message}");
            }

            _BotUpdatesChannel.Writer.Complete();
        }

        /// <summary>
        /// Spawns a Task (Consumer) that is responsible for handling fetched updats
        /// </summary>
        private static async Task _HandleUpdates()
        {
            await Say.Verbose("Task for handling updates started");

            while (await _BotUpdatesChannel.Reader.WaitToReadAsync())
            {
                try
                {
                    var update = await _BotUpdatesChannel.Reader.ReadAsync();
                    await _UpdateHandlerChain.Handle(new UpdateHandlerArgs(_Bot, update));
                }
                catch (UpdateNotHandledException updateNotHandled)
                {
                    await Say.Warning($"Update {updateNotHandled.Update.Id} ({updateNotHandled.Update.Type}) remains unhandled");
                }
            }

            await Say.Verbose("Task for handling updates shuts down");
        }
    }
}
