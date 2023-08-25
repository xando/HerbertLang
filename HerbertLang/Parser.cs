namespace HerbertLang;


public class Parser {

    public static AstNode parse(List<Token> tokens) {
        var parser = new Parser(tokens);
        return parser.parseProgram();
    }

    List<Token> tokens;
    int position;

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        this.position = 0;
    }

    Token? peekToken(int ahead = 0) {
        if (position + ahead < tokens.Count) {
            return tokens[position + ahead];
        }
        return null;
    }

    void consume(string type) {
        if (position >= tokens.Count) {

            var prevToken = tokens[position - 1];
            var msg = string.Format("Expected '{0}'", type);

            throw new LanguageError(msg, prevToken.line, prevToken.column);
        }

        var token = tokens[position];

        if (token.type != type) {
            var msg = string.Format("Expected '{0}' got '{1}'", type, token.type);
            throw new LanguageError(msg, token.line, token.column);
        }

        position++;
    }

    public F_DefinitionNode parseFunctionDefinition() {
        var parameters = new List<F_ParameterNode>();
        var parametersNames = new List<string>();

        consume("#");

        Token? t = peekToken();
        var functionToken = t.Value;

        consume("FUNCTION");

        t = peekToken();
        if (t.HasValue && t.Value.type == "(") {

            consume("(");

            while (peekToken().HasValue && peekToken().Value.type == "PARAMETER") {
                var parameterToken = peekToken().Value;
                var parameter = new F_ParameterNode(parameterToken);

                if (parametersNames.Contains(parameter.name)) {
                    throw new LanguageError("Duplicate parameter", parameter);
                }

                parameters.Add(parameter);
                parametersNames.Add(parameter.name);

                consume("PARAMETER");

                if (peekToken().HasValue && peekToken().Value.type == "," && peekToken(1).Value.type == "PARAMETER") {
                    consume(",");
                } else {
                    break;
                }
            }
            consume(")");
        }

        consume(":");

        var code = parseCode();
        var definition = new F_DefinitionNode(functionToken, parameters, code);

        return definition;
    }

    public ProgramNode parseProgram() {

        var f_definitions = new Dictionary<string, F_DefinitionNode>();
        var mainCodeNode = new CodeNode();

        Token? t = peekToken();
        while (t.HasValue && t.Value.type == "NEW_LINE") {
            consume("NEW_LINE");
            t = peekToken();
        }

        while (t.HasValue) {

            if (t.Value.type == "#") {
                var definition = parseFunctionDefinition();

                if (f_definitions.ContainsKey(definition.name)) {
                    throw new LanguageError(
                        $"Function '{definition.name}' already defined.",
                        definition
                    );
                }
                f_definitions[definition.name] = definition;

                t = peekToken();
                if (t != null) {
                    consume("NEW_LINE");
                }

            }
            else {
                mainCodeNode.extend(parseCode());

                t = peekToken();
                if (t != null) {
                    consume("NEW_LINE");
                }

            }

            t = peekToken();
        }

        return new ProgramNode(f_definitions, mainCodeNode);
    }

    public F_CallNode parseFunctionCall() {
        var arguments = new List<CodeNode>();

        Token? t = peekToken();

        var token = t.Value;

        consume("FUNCTION");

        t = peekToken();

        if (t.HasValue && t.Value.type == "(") {
            consume("(");

            while (peekToken().HasValue && peekToken().Value.type != ")") {

                arguments.Add(parseCode());

                if (peekToken().HasValue && peekToken().Value.type == ",") {
                    consume(",");
                } else {
                    break;
                }
            }
            consume(")");
        }

        return new F_CallNode(token, arguments);
    }

    public CodeNode parseCode() {

        List<AstNode> steps = new List<AstNode>();

        Token? t = peekToken();

        while (t.HasValue && (
            t.Value.type == "STEP" ||
            t.Value.type == "FUNCTION" ||
            t.Value.type == "PARAMETER")) {

            if (t.Value.type == "FUNCTION") {
                steps.Add(parseFunctionCall());

            } else if (t.Value.type == "PARAMETER") {
                steps.Add(parseVariable());


            } else if (t.Value.type == "STEP") {
                steps.Add(parseStep());
            }

            t = peekToken();
        }

        return new CodeNode(steps);
    }

    public StepNode parseStep() {
        Token? t = peekToken();
        consume("STEP");
        return new StepNode(t.Value);
    }

    public VariableNode parseVariable() {
        Token? t = peekToken();
        consume("PARAMETER");
        return new VariableNode(t.Value);
    }

    public F_ParameterNode parseParameter() {
        Token? t = peekToken();
        consume("PARAMETER");
        return new F_ParameterNode(t.Value);
    }
}