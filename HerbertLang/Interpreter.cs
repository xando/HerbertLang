

namespace HerberLanguage
{
    public static class Interpreter
    {
        public static string execute(string code)
        {

            var tokens = Lexer.tokenize(code);
            var parser = new Parser(tokens);

            try
            {
                var program = parser.parseProgram() as Program;
                return program.compile();
            }
            catch (LanguageError ex)
            {
                return ex.ToString();
            }
        }
    }
}