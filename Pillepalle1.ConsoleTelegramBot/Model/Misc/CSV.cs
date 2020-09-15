using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pillepalle1.ConsoleTelegramBot.Model.Misc
{
    internal static class CSV
    {
        public static String[] ParseCsvRecord(String csv, Char delimiter = ',', Char escape = '\"')
        {
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Initialisierung
            List<String> l = new List<String>();
            Char[] record = csv.ToCharArray();

            StringBuilder property = new StringBuilder();
            Char c;

            Boolean escaping = false;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Betrachte jeden Buchstaben einzeln und interpretiere ihn
            for (Int32 i = 0; i < record.Length; i++)
            {
                c = record[i];

                // Escape-Character: Einfach -> Escape; Doppelt -> Anhängen
                if (c == escape)
                {
                    if (i == record.Length - 1)
                    {
                        escaping = !escaping;
                    }
                    else if (escape == record[1 + i])
                    {
                        property.Append(c);
                        i++;
                    }
                    else
                    {
                        escaping = !escaping;
                    }
                }

                // Delimiter: Escaping -> Anhängen; Sonst Datensatz speichern
                else if (c == delimiter)
                {
                    if (escaping)
                    {
                        property.Append(c);
                    }
                    else
                    {
                        l.Add(property.ToString().Trim());
                        property = new StringBuilder();
                    }
                }

                // Jeder andere Buchstabe: Anhängen
                else
                {
                    property.Append(c);
                }
            }

            l.Add(property.ToString().Trim());                  // Letzten Datensatz speichern

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // Konsistenzprüfung und Rückgabe der Datensätze
            if (escaping) throw new FormatException();          // CSV wohlgeformt?

            return l.ToArray<String>();
        }
        public static String CreateCsvRecord(Char delimiter, Char escape, params String[] p)
        {
            StringBuilder record = new StringBuilder();
            String tmp;

            for (Int32 i = 0; i < p.Length; i++)
            {
                if (0 != i) record.Append(delimiter);

                tmp = p[i].Replace(new String(escape, 1), new String(escape, 2));
                if (tmp.Contains(delimiter)) tmp = escape + tmp + escape;

                record.Append(tmp);
            }

            return record.ToString();
        }

        // Object
        public static String CreateCsvRecord(Char delimiter, Char escape, params Object[] p)
        {
            String[] s = new String[p.Length];

            for (Int32 i = 0; i < p.Length; i++)
                s[i] = p[i].ToString();

            return CreateCsvRecord(delimiter, escape, s);
        }

        // Int32
        public static String CreateCsvRecord(Char delimiter, Char escape, String format, params Int32[] p)
        {
            String[] s = new String[p.Length];

            for (Int32 i = 0; i < p.Length; i++)
                s[i] = p[i].ToString(format);

            return CreateCsvRecord(delimiter, escape, s);
        }
        public static String CreateCsvRecord(Char delimiter, Char escape, params Int32[] p)
        {
            return CreateCsvRecord(delimiter, escape, "", p);
        }

        // Double
        public static String CreateCsvRecord(Char delimiter, Char escape, String format, params Double[] p)
        {
            String[] s = new String[p.Length];

            for (Int32 i = 0; i < p.Length; i++)
                s[i] = p[i].ToString(format);

            return CreateCsvRecord(delimiter, escape, s);
        }
        public static String CreateCsvRecord(Char delimiter, Char escape, params Double[] p)
        {
            return CreateCsvRecord(delimiter, escape, "", p);
        }
    
    }
}
