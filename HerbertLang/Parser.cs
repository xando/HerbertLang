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

    Token? peek(int ahead = 0) {
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

        Token? t = peek();

        var token = t.Value;

        consume("FUNCTION");

        t = peek();
        if (t.HasValue && t.Value.type == "(") {

            consume("(");

            while (peek().HasValue && peek().Value.type == "PARAMETER") {
                var parameterToken = peek().Value;
                var parameter = new F_ParameterNode(parameterToken);

                if (parametersNames.Contains(parameter.name)) {
                    throw new LanguageError("Duplicate parameter", parameter);
                }

                parameters.Add(parameter);
                parametersNames.Add(parameter.name);

                consume("PARAMETER");

                if (peek().HasValue && peek().Value.type == "," && peek(1).Value.type == "PARAMETER") {
                    consume(",");
                } else {
                    break;
                }
            }
            consume(")");
        }
        consume(":");
        var code = parseCode();
        var definition = new F_DefinitionNode(token, parameters, code);
        return definition;
    }

    public ProgramNode parseProgram() {

        var f_definitions = new Dictionary<string, F_DefinitionNode>();
        var mainCodeNode = new CodeNode();

        Token? t = peek();

        while (t.HasValue) {

            if (t.Value.type == "NEW_LINE") {
                consume("NEW_LINE");
            } else
            if (t.Value.type == "FUNCTION" &&
                peek(1).HasValue && peek(1).Value.type == ":") {
                var definition = parseFunctionDefinition();

                if (f_definitions.ContainsKey(definition.name)) {
                    throw new LanguageError(
                        $"Function '{definition.name}' already defined.",
                        definition
                    );
                }
                f_definitions[t.Value.content] = definition;
            } else {
                mainCodeNode.extend(parseCode());
            }
            t = peek();

        }

        return new ProgramNode(f_definitions, mainCodeNode);
    }

    public F_CallNode parseFunctionCall() {
        var arguments = new List<CodeNode>();

        Token? t = peek();

        var token = t.Value;

        consume("FUNCTION");

        t = peek();

        if (t.HasValue && (
            t.Value.type == "=" ||
            t.Value.type == "+" ||
            t.Value.type == "-")) {

            if (t.Value.type == "+") {
                consume("+");
                var number = parseNumber();
                // durability = number.value;
            }
            if (t.Value.type == "-") {
                consume("-");
                var number = parseNumber();
            }
        }

        if (t.HasValue && t.Value.type == "(") {
            consume("(");

            while (peek().HasValue && peek().Value.type != ")") {

                arguments.Add(parseCode());

                if (peek().HasValue && peek().Value.type == ",") {
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

        Token? t = peek();

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

            t = peek();
        }

        return new CodeNode(steps);
    }

    public StepNode parseStep() {
        Token? t = peek();
        consume("STEP");
        return new StepNode(t.Value);
    }

    public VariableNode parseVariable() {
        Token? t = peek();
        consume("PARAMETER");
        return new VariableNode(t.Value);
    }

    public NumberNode parseNumber() {
        Token? t = peek();

        var sign = 1;

        while (t?.type == "-") {
            sign *= -1;
            consume("-");
            t = peek();
        }
        var numer = Int32.Parse(t?.content);

        consume("NUMBER");

        return new NumberNode(numer * sign);

    }
}