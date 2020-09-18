using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public sealed class StringTokenizer
    {
        private static readonly Regex _Whitespaces = new Regex("\\s+", RegexOptions.Compiled);
        private string _sourceString = null;                            // Provided data to split

        /// <summary>
        /// Creates a new StringTokenizer
        /// </summary>
        /// <param name="dataInput">Data to be split into tokens</param>
        public StringTokenizer(string dataInput)
        {
            _sourceString = dataInput ?? throw new NullReferenceException("Cannot tokenize null");
        }

        /// <summary>
        /// Access tokens by index
        /// </summary>
        public string this[int index]
        {
            get
            {
                if (index >= this.Count)
                {
                    return String.Empty;
                }

                return _Tokens[index];
            }
        }

        /// <summary>
        /// How many tokens does the command consist of
        /// </summary>
        public int Count
        {
            get
            {
                return _Tokens.Count;
            }
        }

        /// <summary>
        /// Which strategy is used to split the string into tokens
        /// </summary>
        public StringTokenizerStrategy Strategy
        {
            get
            {
                return _strategy;
            }
            set
            {
                if (value != _strategy)
                {
                    _strategy = value;
                    _tokens = null;
                }
            }
        }
        private StringTokenizerStrategy _strategy = StringTokenizerStrategy.Split;

        /// <summary>
        /// Character used to delimit tokens
        /// </summary>
        public char Delimiter
        {
            get
            {
                return _delimiter;
            }
            set
            {
                if (value != _delimiter)
                {
                    _delimiter = value;
                    _tokens = null;
                }
            }
        }
        private char _delimiter = ' ';

        /// <summary>
        /// Character used to escape the following character
        /// </summary>
        public char Escape
        {
            get
            {
                return _escape;
            }
            set
            {
                if (value != _escape)
                {
                    _escape = value;

                    if (Strategy == StringTokenizerStrategy.Escaping)
                    {
                        _tokens = null;
                    }
                }
            }
        }
        private char _escape = '\\';

        /// <summary>
        /// Character used to surround tokens
        /// </summary>
        public char Quotes
        {
            get
            {
                return _quotes;
            }
            set
            {
                if (value != _quotes)
                {
                    _quotes = value;

                    if (Strategy == StringTokenizerStrategy.Quotation)
                    {
                        _tokens = null;
                    }
                }
            }
        }
        private char _quotes = '"';

        /// <summary>
        /// Formats and splits the tokens by delimiter allowing to add delimiters by quoting
        /// </summary>
        private List<string> _SplitRespectingQuotation()
        {
            string data = _sourceString;

            // Doing some basic transformations
            data = _Whitespaces.Replace(data, " ");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            List<string> l = new List<string>();
            char[] record = data.ToCharArray();

            StringBuilder property = new StringBuilder();
            char c;

            bool quoting = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            for (int i = 0; i < record.Length; i++)
            {
                c = record[i];

                // Quotation-Character: Single -> Quote; Double -> Append
                if (c == Quotes)
                {
                    if (i == record.Length - 1)
                    {
                        quoting = !quoting;
                    }
                    else if (Quotes == record[1 + i])
                    {
                        property.Append(c);
                        i++;
                    }
                    else
                    {
                        quoting = !quoting;
                    }
                }

                // Delimiter: Escaping -> Append; Otherwise append
                else if (c == Delimiter)
                {
                    if (quoting)
                    {
                        property.Append(c);
                    }
                    else
                    {
                        l.Add(property.ToString());
                        property.Clear();
                    }
                }

                // Any other character: Append
                else
                {
                    property.Append(c);
                }
            }

            l.Add(property.ToString());                         // Add last token

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Checking consistency
            if (quoting) throw new FormatException();          // All open quotation marks closed

            return l;
        }

        /// <summary>
        /// Splits the string by declaring one character as escape
        /// </summary>
        private List<string> _SplitRespectingEscapes()
        {
            // Doing some basic transformations
            string data = _Whitespaces.Replace(_sourceString, " ");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            List<string> tokenList = new List<string>();
            StringBuilder tokenBuilder = new StringBuilder();

            bool escapeNext = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in data.ToCharArray())
            {
                if (escapeNext)
                {
                    tokenBuilder.Append(c);
                    escapeNext = false;
                    continue;
                }

                if (c == Escape)
                {
                    escapeNext = true;
                }
                else if (c == Delimiter)
                {
                    tokenList.Add(tokenBuilder.ToString());
                    tokenBuilder.Clear();
                }
                else
                {
                    tokenBuilder.Append(c);
                }
            }

            return tokenList;
        }

        /// <summary>
        /// Splits the string by calling a simple String.Split
        /// </summary>
        private List<string> _SplitPlain()
        {
            return new List<string>(_Whitespaces.Replace(_sourceString, " ").Split(Delimiter));
        }

        /// <summary>
        /// Backer for tokens
        /// </summary>
        private List<string> _Tokens
        {
            get
            {
                if (null == _tokens)
                {
                    switch (Strategy)
                    {
                        case (StringTokenizerStrategy.Quotation): _tokens = _SplitRespectingQuotation(); break;
                        case (StringTokenizerStrategy.Escaping): _tokens = _SplitRespectingEscapes(); break;

                        default: _tokens = _SplitPlain(); break;
                    }
                }

                return _tokens;
            }
        }
        private List<string> _tokens = null;
    }

    public enum StringTokenizerStrategy
    {
        Split,
        Quotation,
        Escaping
    }
}
