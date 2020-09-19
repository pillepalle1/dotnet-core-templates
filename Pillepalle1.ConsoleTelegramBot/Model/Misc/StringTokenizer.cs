using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    public static class StringTokenizer
    {
        /// <summary>
        /// Formats and splits the tokens by delimiter allowing to add delimiters by quoting
        /// </summary>
        public static ImmutableList<string> SplitRespectingQuotation(this string sourceString, char delimiter = ' ', char quotes = '"')
        {
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            var tokenList = ImmutableList<string>.Empty;
            var tokenBuilder = new StringBuilder();

            var expectingDelimiterOrQuotes = false;     // Next char must be Delimiter or Quotes
            var hasReadTokenChar = false;               // We are not between tokens (=> No quoting)
            var isQuoting = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in sourceString)
            {
                if (expectingDelimiterOrQuotes)
                {
                    expectingDelimiterOrQuotes = false;

                    if (c == delimiter)
                    {
                        isQuoting = false;
                    }

                    else if (c == quotes)
                    {
                        tokenBuilder.Append(c);
                        continue;
                    }

                    else
                    {
                        throw new FormatException($"Unexpected single use of quotes ({quotes}) in quoted token");
                    }
                }

                // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --

                if (c == quotes)
                {
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

                else if (c == delimiter)
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

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Tidy up open flags and checking consistency

            tokenList = tokenList.Add(tokenBuilder.ToString());

            if (isQuoting && !expectingDelimiterOrQuotes)
            {
                throw new FormatException("Missing closing quotes");
            }

            return tokenList;
        }

        /// <summary>
        /// Splits the string by declaring one character as escape
        /// </summary>
        public static ImmutableList<string> SplitRespectingEscapes(this string sourceString, char delimiter = ' ', char escapeChar = '\\')
        {
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisation
            var tokenList = ImmutableList<string>.Empty;
            var tokenBuilder = new StringBuilder();

            var escapeNext = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Scan character by character
            foreach (char c in sourceString)
            {
                if (escapeNext)
                {
                    tokenBuilder.Append(c);
                    escapeNext = false;
                    continue;
                }

                if (c == escapeChar)
                {
                    escapeNext = true;
                }
                else if (c == delimiter)
                {
                    tokenList = tokenList.Add(tokenBuilder.ToString());
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
        public static ImmutableList<string> SplitPlain(this string sourceString, char delimiter = ' ')
        {
            return ImmutableList<string>.Empty.AddRange(sourceString.Split(delimiter));
        }
    }
}
