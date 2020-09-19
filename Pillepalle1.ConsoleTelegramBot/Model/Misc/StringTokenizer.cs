using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
        private List<string> _SplitRespectingQuotation()
        {
            // Doing some basic transformations
            var data = _Whitespaces.Replace(_sourceString, " ");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            var tokenList = new List<string>();
            var tokenBuilder = new StringBuilder();

            var expectingDelimiter = false;             // Next char must be Delimiter
            var expectingQuotes = false;                // Next char must be Quotes
            var expectingDelimiterOrQuotes = false;     // Next char must be Delimiter or Quotes

            var hasReadTokenChar = false;               // We are not between tokens (=> No quoting)
            var hasSeenSingleQuotes = false;            // We have observed a single quoting char
            var isQuotedToken = false;                  // The token is encapsuled in quotes


            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in data)
            {
                // Occasionally we know what the next character is expected to be. Filter out these
                // cases first
                if (expectingDelimiter)
                {
                    if (c != Delimiter)
                    {
                        throw new FormatException($"Expected delimiter ({Delimiter}) but found {c}");
                    }

                    expectingDelimiter = false;
                }

                if (expectingQuotes)
                {
                    if (c != Quotes)
                    {
                        throw new FormatException($"Expected quotes ({Quotes}) but found {c}");
                    }

                    expectingQuotes = false;
                }

                if (expectingDelimiterOrQuotes)
                {
                    if ((c != Delimiter) && (c != Quotes))
                    {
                        throw new FormatException($"Expected delimiter ({Delimiter}) or quotes ({Quotes}) but found {c}");
                    }

                    expectingDelimiterOrQuotes = false;
                }

                // In case of Quotes, we are faced with the question whether they
                // 1. Open quotation
                // 2. Should be appended to the token
                // 3. Close quotation
                // 4. Are an invalid single character within a token
                if (c == Quotes)
                {
                    if (hasSeenSingleQuotes)
                    {
                        tokenBuilder.Append(c);
                    }

                    hasSeenSingleQuotes = !hasSeenSingleQuotes;

                    // If we are in the middle of a token, we expect either another quotes character
                    // to complete double quotes or a delimiter to end the quotes
                    if (hasReadTokenChar)
                    {
                        if (hasSeenSingleQuotes)
                        {
                            if (isQuotedToken)
                            {
                                expectingDelimiterOrQuotes = true;
                            }
                            else
                            {
                                expectingQuotes = true;
                            }
                        }
                    }
                }

                // In case of a delimiter, we are faced with the question whether to
                // 1. Append the currently parsed token
                // 2. Append the delimiter to the token
                else if (c == Delimiter)
                {
                    if (isQuotedToken)
                    {
                        if (hasSeenSingleQuotes)
                        {
                            tokenList.Add(tokenBuilder.ToString());
                            tokenBuilder.Clear();
                            hasReadTokenChar = false;
                            hasSeenSingleQuotes = false;
                            isQuotedToken = false;
                        }
                        else
                        {
                            tokenBuilder.Append(c);
                            hasReadTokenChar = true;
                        }
                    }
                    else
                    {
                        if (hasSeenSingleQuotes)
                        {
                            if (!hasReadTokenChar)
                            {
                                isQuotedToken = true;
                            }
                            else
                            {
                                throw new FormatException($"Unexpected quotes in token. Did you mean {Quotes}{Quotes}?");
                            }

                            hasSeenSingleQuotes = false;

                            tokenBuilder.Append(c);
                            hasReadTokenChar = true;
                        }
                        else
                        {
                            tokenList.Add(tokenBuilder.ToString());
                            tokenBuilder.Clear();
                            hasReadTokenChar = false;
                        }
                    }
                }

                // Any other character is just being appended to
                else
                {
                    // Single quoting chars can only be used at the end of a token
                    if (hasSeenSingleQuotes)
                    {
                        if (!hasReadTokenChar)
                        {
                            isQuotedToken = true;
                        }
                        else
                        {
                            throw new FormatException($"Unexpected quotes in token. Did you mean {Quotes}{Quotes}?");
                        }

                        hasSeenSingleQuotes = false;
                    }

                    // Append char
                    tokenBuilder.Append(c);
                    hasReadTokenChar = true;
                }
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Tidy up open flags and checking consistency
            if (hasReadTokenChar)
            {
                tokenList.Add(tokenBuilder.ToString());                         // Add last token
                hasReadTokenChar = false;
                expectingDelimiterOrQuotes = false;
                expectingDelimiter = false;
            }

            if (isQuotedToken)
            {
                if (!hasSeenSingleQuotes)
                {
                    throw new FormatException($"Missing matching quotes ({Quotes}) at end of token");
                }

                hasSeenSingleQuotes = false;
            }

            if (hasSeenSingleQuotes)
            {
                throw new FormatException($"Unexpected unmatched quotes ({Quotes}) at end of source string");
            }

            if (expectingDelimiter)
            {
                throw new FormatException($"Reached end of record while expecting delimiter ({Delimiter})");
            }

            if (expectingQuotes)
            {
                throw new FormatException($"Reached end of record while expecting quotes ({Quotes})");
            }

            if (expectingDelimiterOrQuotes)
            {
                throw new FormatException($"Reached end of record while expecting quotes ({Quotes}) or delimiter ({Delimiter})");
            }

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
