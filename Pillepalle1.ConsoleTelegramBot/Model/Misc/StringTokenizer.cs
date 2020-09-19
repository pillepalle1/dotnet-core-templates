using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public sealed class StringTokenizer
    {
        // Static "members"
        private static readonly Regex _Whitespaces = new Regex("\\s+", RegexOptions.Compiled);

        // Members
        private string _sourceString = null;

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
        private ImmutableList<string> _SplitRespectingQuotation()
        {
            // Doing some basic transformations
            var data = _Whitespaces.Replace(_sourceString, " ");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            var tokenList = ImmutableList<string>.Empty;
            var tokenBuilder = new StringBuilder();

            var expectingDelimiterOrQuotes = false;     // Next char must be Delimiter or Quotes
            var hasReadTokenChar = false;               // We are not between tokens (=> No quoting)
            var isQuoting = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in data)
            {
                if (expectingDelimiterOrQuotes)
                {
                    expectingDelimiterOrQuotes = false;

                    // Assertion failed
                    if ((c != Delimiter) && (c != Quotes))
                    {
                        throw new FormatException();
                    }

                    if (c == Delimiter)
                    {
                        isQuoting = false;
                    }

                    if(c == Quotes)
                    {
                        tokenBuilder.Append(c);
                        continue;
                    }
                }

                // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --

                if (c == Quotes)
                {
                    // The code becomes a bit verbose because of sequences like """""""test"""""""
                    // in a foreach-loop

                    // As long as character other than quotes has been added to the token
                    if (!hasReadTokenChar)
                    {
                        // If this was the 2n-th token, add a quote-character
                        if (isQuoting)
                        {
                            tokenBuilder.Append(c);
                        }

                        // We are still about to decide whether we are quoting or not
                        isQuoting = !isQuoting;
                    }
                    else
                    {
                        if (isQuoting)
                        {
                            expectingDelimiterOrQuotes = true;
                        }
                        else
                        {
                            tokenBuilder.Append(c);
                        }
                    }
                }

                else if (c == Delimiter)
                {
                    if (isQuoting)
                    {
                        tokenBuilder.Append(c);

                        hasReadTokenChar = true;
                    }
                    else
                    {
                        tokenList = tokenList.Add(tokenBuilder.ToString());
                        tokenBuilder.Clear();

                        hasReadTokenChar = false;
                    }
                }

                // Any other character is just being appended to
                else
                {
                    tokenBuilder.Append(c);

                    hasReadTokenChar = true;
                }
            }

            // Add last token
            tokenList = tokenList.Add(tokenBuilder.ToString());


            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Tidy up open flags and checking consistency
            return tokenList;
        }

        /// <summary>
        /// Splits the string by declaring one character as escape
        /// </summary>
        private List<string> _SplitRespectingEscapes()
        {
            // Doing some basic transformations
            var data = _Whitespaces.Replace(_sourceString, " ");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            var tokenList = new List<string>();
            var tokenBuilder = new StringBuilder();

            var escapeNext = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in data)
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

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Checking consistency
            if (escapeNext) throw new FormatException();            // Expecting additional char

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
                        case (StringTokenizerStrategy.Quotation): _tokens = new List<string>(_SplitRespectingQuotation()); break;
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
