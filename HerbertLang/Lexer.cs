using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HerberLanguage {

    public struct Token
    {
        public string content;
        public string type;
        public int column;
        public int line;

        public Token(string type, string content, int line, int column) {
            this.type = type;
            this.content = content;
            this.line = line;
            this.column = column;
        }

        public override string ToString()
        {
            return string.Format("<Token: {0}, '{1}', {2}:{3}>", content, type, line, column);
        }
    }

    public static class Lexer {

        static Dictionary<string, Regex> rules = new Dictionary<string, Regex>() {
            {"NEW_LINE", new Regex(@"^\n")},
            {":", new Regex(@"^\:")},
            {"[", new Regex(@"^\[")},
            {"]", new Regex(@"^\]")},
            {"(", new Regex(@"^\(")},
            {")", new Regex(@"^\)")},
            {",", new Regex(@"^\,")},
            {"-", new Regex(@"^\-")},
            {"+", new Regex(@"^\+")},
            {"=", new Regex(@"^\=")},
            {"STEP", new Regex(@"^(s|l|r)")},
            {"NUMBER", new Regex(@"^[0-9]+")},
            {"PARAMETER", new Regex(@"^[A-Z]")},
            {"FUNCTION", new Regex(@"^(a|b|c|d|e|f|g|h|i|j|k|m|n|o|p|q|t|u|v|w|x|y|z)")}
        };

        public static List<Token> tokenize(string code) {

            var line = 1;
            var column = 1;
            var index = 0;

            var tokens = new List<Token>();
            while (index < code.Length) {
                if (code[index] == ' ') {
                    index++;
                    column++;
                    continue;
                }
                var left = code.Substring(index);
                var found = false;
                foreach (var rule in rules) {
                    Match match = rule.Value.Match(left);

                    if (match.Success) {
                        found = true;
                        var token = new Token(rule.Key, match.Value, line, column);
                        tokens.Add(token);
                        index += match.Length;
                        if (rule.Key == "NEW_LINE") {
                            line += 1;
                            column = 1;
                        } else {
                            column += match.Length;
                        }
                        break;
                    }
                }
                if (!found) {
                    var msg = string.Format("Unexpected character '{0}'", code[index]);
                    throw new LanguageError(msg, line, column);
                }
            }
            return tokens;
        }
    }
}