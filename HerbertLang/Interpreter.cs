using System.Collections.Generic;
using System.Text;


namespace HerberLang
{
    public static class Interpreter
    {

        public static List<ExecutionStep> evalToCode(string program) {
            var tokens = Lexer.tokenize(program);
            var ast = Parser.parse(tokens);

            var code = ast.eval() as Code;
            var steps = new List<ExecutionStep>();

            if (code == null) return steps;

            foreach (Step step in code.steps) {
                steps.Add(step.step_);
            }

            return steps;
        }

        public static string evalToString(string program)
        {
            try {
                var tokens = Lexer.tokenize(program);
                var ast = Parser.parse(tokens);

                var code = ast.eval() as Code;

                if (code == null) return "";

                var steps = new StringBuilder();

                if (code != null) {
                    foreach (Step step in code.steps) {
                        steps.Append(step.step);
                    }
                }

                return steps.ToString();

            } catch (LanguageError ex) {
                return ex.ToString();
            }
        }
    }
}