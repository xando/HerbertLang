using System.Collections.Generic;
using System.Text;


namespace HerberLang {
    public static class Interpreter {

        public static List<Step> evalToCode(string program) {
            var tokens = Lexer.tokenize(program);
            var ast = Parser.parse(tokens);

            var code = ast.eval() as CodeNode;
            var steps = new List<Step>();

            if (code == null) return steps;

            foreach (StepNode step in code.steps) {
                steps.Add(step.step_);
            }

            return steps;
        }

        public static string evalToString(string program) {
            try {
                var tokens = Lexer.tokenize(program);
                var ast = Parser.parse(tokens);

                var code = ast.eval() as CodeNode;

                if (code == null) return "";

                var steps = new StringBuilder();

                if (code != null) {
                    foreach (StepNode step in code.steps) {
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