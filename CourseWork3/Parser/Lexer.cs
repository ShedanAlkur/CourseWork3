using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseWork3.Parser
{
    class Lexer
    {
        private static string splitToTokensPattern
            = @"(?:\/\/.*$)|[A-Za-z](?:[A-Za-z0-9_])*|\d+(?:\,\d+)?|[*\/+-]|\," + "|\\\".*\\\"" + @"|\(|\)";


        public static string[] SplitToTokens(string input)
        {
            string[] tokens = Regex.Matches(input, splitToTokensPattern)
                .Cast<Match>()
                .Select(match => match.Value)
                .Select(x => (x.StartsWith("\"") && x.EndsWith("\"")) ? x : x.ToLower())
                .Where(x => !x.StartsWith(@"//"))
                .ToArray();

            return tokens;
        }

        public static string[] SplitToTokensFromFile(string path)
        {
            List<string> tokens = new List<string>();

            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    tokens.AddRange(SplitToTokens(line));
                    tokens.Add(Keywords.EOL);
                }
                tokens.Add(Keywords.EOF);
            }

            return tokens.ToArray();
        }
    }
}
