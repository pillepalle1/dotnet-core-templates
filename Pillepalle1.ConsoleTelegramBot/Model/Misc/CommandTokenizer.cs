namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public class CommandTokenizer
    {
        private StringTokenizer _stringTokenizer = null;                // Responsible for splitting string

        /// <summary>
        /// This class splits a user entered command string into tokens used for automated processing
        /// </summary>
        public CommandTokenizer(string commandString)
        {
            _stringTokenizer = new StringTokenizer(commandString, StringTokenizerStrategy.Quotation)
            {
                Delimiter = ' ',
                Escape = '\\',
                Quotes = '"'
            };
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
                    int indexAt = _stringTokenizer[0].IndexOf('@');
                    
                    if(0 > indexAt)
                    {
                        indexAt = _stringTokenizer[0].Length;
                    }

                    return _stringTokenizer[0].Substring(0, indexAt);
                }
                else
                {
                    return _stringTokenizer[index];
                }
            }
        }

        /// <summary>
        /// Returns the total number of Tokens
        /// </summary>
        public int Count
        {
            get
            {
                return _stringTokenizer.Count;
            }
        }
    }
}
