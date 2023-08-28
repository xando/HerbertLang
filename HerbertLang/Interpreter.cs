using System.Text;

namespace HerbertLang;

public static class Interpreter {

    public static (string code, string error) eval(string programText) {

        string code = "";
        string error = "";
        
        try {
            var tokens = Lexer.tokenize(programText);
            var programNode = Parser.parse(tokens);

            var codeNode = (CodeNode)programNode.eval();

            var steps = new StringBuilder();

            foreach (StepNode stepNode in codeNode.steps) {
                steps.Append(stepNode.step);
            }

            code = steps.ToString();

        } catch (LanguageError ex) {
            error = ex.ToString();
        }

        return (code, error);
    }
}
