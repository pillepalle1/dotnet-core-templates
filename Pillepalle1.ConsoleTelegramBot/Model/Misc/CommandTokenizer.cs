using System;
using System.Text.RegularExpressions;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public sealed class CommandTokenizer
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a CommandTokenizer
        /// </summary>
        /// <param name="s"></param>
        public CommandTokenizer(string s)
        {
            this.Tokens = this.LoadString(s);
        }
        #endregion

        #region Interface
        /// <summary>
        /// Access tokens by index
        /// </summary>
        public string this[int index]
        {
            get
            {
                if (index >= this.Tokens.Length)
                {
                    return String.Empty;
                }

                return this.Tokens[index];
            }
        }

        /// <summary>
        /// How many tokens does the command consist of
        /// </summary>
        public int Count
        {
            get
            {
                return this.Tokens.Length;
            }
        }
        #endregion

        #region Predefined Regex
        private Regex Whitespaces
        {
            get
            {
                return new Regex("\\s+");
            }
        }
        #endregion

        #region Implementation Details
        /// <summary>
        /// Formats and splits the tokens
        /// </summary>
        private string[] LoadString(string line)
        {
            string data = line;

            // Doing some basic transformations
            data = Whitespaces.Replace(data, " ");

            // Splitting string into tokens
            return CSV.ParseCsvRecord(data, ' ');
        }

        /// <summary>
        /// Backer for tokens
        /// </summary>
        private string[] Tokens
        {
            get;
            set;
        }
        #endregion
    }
}
