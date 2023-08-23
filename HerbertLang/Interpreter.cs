using System.Text;

namespace HerbertLang;


public static class Interpreter {

    private static CodeNode eval(string program) {

        var tokens = Lexer.tokenize(program);
        var programNode = Parser.parse(tokens);

        return (CodeNode)programNode.eval();
    }

    public static string evalToCode(string program) {
        var code = Interpreter.eval(program);

        var steps = new List<char>();

        // if (code == null) return "";

        // foreach (StepNode step in code.steps) {
        //     steps.Add(step.step_);
        // }

        return new string(steps.ToArray());
    }

    public static string evalToString(string programText) {
        try {
            var codeNode = Interpreter.eval(programText);

            var steps = new StringBuilder();

            foreach (StepNode stepNode in codeNode.steps) {
                steps.Append(stepNode.step);
            }

            return steps.ToString();

        } catch (LanguageError ex) {
            return ex.ToString();
        }
    }
}
