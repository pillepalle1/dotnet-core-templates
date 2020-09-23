using System.Collections.Immutable;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public class CommandTokenizer
    {
        private ImmutableList<string> _tokens = null;

        /// <summary>
        /// This class splits a user entered command string into tokens used for automated processing
        /// </summary>
        public CommandTokenizer(string commandString)
        {
            _tokens = commandString.SplitRespectingQuotation(' ', '"');
        }

        /// <summary>
        /// Allows to access Tokens by index. Instantiated objects can be used like an array
        /// </summary>
        /// <param name="index">Zero based index of the token</param>
        public string this[int index]
        {
            get
            {
                // For some reason Telegram sometimes adds the @bot_name after the command
                // which always results in a "Unknown-Command-Exception". This snippet gets
                // rid of it
                if (0 == index)
                {
                    int indexAt = _tokens[0].IndexOf('@');
                    
                    if(0 > indexAt)
                    {
                        indexAt = _tokens[0].Length;
                    }

                    return _tokens[0].Substring(0, indexAt);
                }
                else
                {
                    return _tokens[index];
                }
            }
        }

        /// <summary>
        /// Returns the total number of Tokens
        /// </summary>
        public int Count => _tokens.Count;
    }
}
