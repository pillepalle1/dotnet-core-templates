using System;
using System.Text;
using System.Collections.Immutable;

namespace pillepalle1.Text
{
    public static class StringTokenizer
    {
        private static FormatException _nonQuotedTokenMayNotContainQuotes =
            new FormatException("[RFC4180] If fields are not enclosed with double quotes, then double quotes may not appear inside the fields.");

        private static FormatException _quotesMustBeEscapedException =
            new FormatException("[RFC4180] If double-quotes are used to enclose fields, then a double-quote appearing inside a field must be escaped by preceding it with another double quote.");

        private static FormatException _tokenNotFullyEnclosed =
            new FormatException("[RFC4180] \"Each field may or may not be enclosed in double quotes\". However, for the final field the closing quotes are missing.");


        /// <summary>
        /// <para>
        /// Formats and splits the tokens by delimiter allowing to add delimiters by quoting 
        /// similar to https://tools.ietf.org/html/rfc4180
        /// </para>
        /// 
        /// <para>
        /// Each field may or may not be enclosed in double quotes (however some programs, such as 
        /// Microsoft Excel, do not use double quotes at all). If fields are not enclosed with 
        /// double quotes, then double quotes may not appear inside the fields.
        /// </para>
        /// 
        /// <para>
        /// Fields containing line breaks (CRLF), double quotes, and commas should be enclosed in 
        /// double-quotes.
        /// </para>
        /// 
        /// <para>
        /// If double-quotes are used to enclose fields, then a double-quote appearing inside a 
        /// field must be escaped by preceding it with another double quote.
        /// </para>
        /// 
        /// <para>
        /// The ABNF defines 
        /// 
        /// [field = (escaped / non-escaped)] ||  
        /// [non-escaped = *TEXTDATA] || 
        /// [TEXTDATA =  %x20-21 / %x23-2B / %x2D-7E]
        /// 
        /// specifically forbidding to include quotes in non-escaped fields, hardening the *SHOULD*
        /// requirement above.
        /// </para>
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
                        hasReadTokenChar = true;
                        continue;
                    }

                    else
                    {
                        throw _quotesMustBeEscapedException;
                    }
                }

                // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --

                if (c == quotes)
                {
                    if (isQuoting)
                    {
                        expectingDelimiterOrQuotes = true;
                    }

                    else
                    {
                        if (hasReadTokenChar)
                        {
                            throw _nonQuotedTokenMayNotContainQuotes;
                        }

                        isQuoting = true;
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
                throw _tokenNotFullyEnclosed;
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
            // Tidy up open flags and checking consistency
            tokenList = tokenList.Add(tokenBuilder.ToString());

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
