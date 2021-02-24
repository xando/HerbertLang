using System.Collections.Generic;
using System.Text;


namespace HerberLang {
    public static class Interpreter {

        private static CodeNode eval(string program) {

            var tokens = Lexer.tokenize(program);
            var ast = Parser.parse(tokens);

            return ast.eval() as CodeNode;
        }

        public static string evalToCode(string program) {
            var code = Interpreter.eval(program);

            var steps = new List<char>();

            if (code == null) return "";

            foreach (StepNode step in code.steps) {
                steps.Add(step.step_);
            }

            return new string(steps.ToArray());
        }

        public static string evalToString(string program) {
            try {
                var code = Interpreter.eval(program);

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