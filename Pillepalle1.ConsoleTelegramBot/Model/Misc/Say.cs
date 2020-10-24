using System;
using System.Threading.Tasks;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public static class Say
    {
        public static int Verbosity = 0;

        /// <summary>
        /// Prints a string to stderr if minimum verbosity is reached
        /// </summary>
        /// <param name="message">String to write</param>
        /// <param name="minVerbosity">Verbosity threshold for writing</param>
        public static async Task Verbose(string message, int minVerbosity = 0)
        {
            if (Verbosity >= minVerbosity)
            {
                await Console.Error.WriteLineAsync(message);
            }
        }

        /// <summary>
        /// Prints a string to stderr (in green)
        /// </summary>
        /// <param name="message"></param>
        public static async Task Success(string message)
        {
            ConsoleColor fgColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            await Console.Error.WriteLineAsync(message);
            Console.ForegroundColor = fgColor;
        }

        /// <summary>
        /// Prints a string to stderr (in yellow)
        /// </summary>
        /// <param name="message">String to write</param>
        public static async Task Warning(string message)
        {
            ConsoleColor fgColor = Console.ForegroundColor;
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            await Console.Error.WriteLineAsync(message);
            Console.ForegroundColor = fgColor;
        }

        /// <summary>
        /// Prints a string to stderr (in red)
        /// </summary>
        /// <param name="message">String to write</param>
        public static async Task Error(string message)
        {
            ConsoleColor fgColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync(message);
            Console.ForegroundColor = fgColor;
        }
    }
}
