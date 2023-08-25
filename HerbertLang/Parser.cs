namespace HerbertLang;
#nullable enable

public class Parser {

    public static AstNode parse(List<Token> tokens) {
        var parser = new Parser(tokens);
        return parser.parseProgram();
    }

    List<Token> tokens;
    int position;

    public Token? currentToken {
        get {
            if (position < tokens.Count) {
                return tokens[position];
            }
            return null;
        }
    }

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        this.position = 0;
    }

    void consume(string type) {
        if (this.position >= tokens.Count) {

            var prevToken = tokens[this.position - 1];
            var msg = string.Format("Expected '{0}'", type);

            throw new LanguageError(msg, prevToken.line, prevToken.column);
        }

        var token = tokens[position];

        if (token.type != type) {
            var msg = string.Format("Expected '{0}' got '{1}'", type, token.type);
            throw new LanguageError(msg, token.line, token.column);
        }

        this.position++;
    }

    public F_DefinitionNode parseFunctionDefinition() {
        var parameters = new List<F_ParameterNode>();
        var parametersNames = new List<string>();

        consume("#");

        var functionNameToken = this.currentToken!.Value;

        consume("FUNCTION");

        if (this.currentToken != null && this.currentToken.Value.type == "(") {

            consume("(");

            while (this.currentToken != null && this.currentToken.Value.type == "PARAMETER") {
                
                var parameter = new F_ParameterNode(this.currentToken!.Value);

                if (parametersNames.Contains(parameter.name)) {
                    throw new LanguageError("Duplicate parameter", parameter);
                }

                parameters.Add(parameter);
                parametersNames.Add(parameter.name);

                consume("PARAMETER");

                if (this.currentToken != null && this.currentToken.Value.type == ",") {
                    consume(",");
                    // TODO - not sure if we should allow trailing commas.
                    //        Check if there is a parameter after the comma, if not, throw an error.
                    //        Technically we should expect that there is a parameter after the comma.
                }
            }
            consume(")");
        }

        consume(":");

        var code = parseCode();
        var definition = new F_DefinitionNode(functionNameToken, parameters, code);

        return definition;
    }

    public ProgramNode parseProgram() {

        var f_definitions = new Dictionary<string, F_DefinitionNode>();
        var mainCodeNode = new CodeNode();

        while (this.currentToken != null && this.currentToken.Value.type == "NEW_LINE") {
            consume("NEW_LINE");
        }

        while (this.currentToken != null) {

            if (this.currentToken.Value.type == "#") {
                var definition = parseFunctionDefinition();

                if (f_definitions.ContainsKey(definition.name)) {
                    throw new LanguageError(
                        $"Function '{definition.name}' already defined.",
                        definition
                    );
                }
                f_definitions[definition.name] = definition;

                if (this.currentToken != null) {
                    consume("NEW_LINE");
                }

            }
            else {
                mainCodeNode.extend(parseCode());

                if (this.currentToken != null) {
                    consume("NEW_LINE");
                }

            }
        }

        return new ProgramNode(f_definitions, mainCodeNode);
    }

    public F_CallNode parseFunctionCall() {
        var arguments = new List<CodeNode>();
    
        var token = this.currentToken!.Value;

        consume("FUNCTION");

        if (this.currentToken != null && this.currentToken.Value.type == "(") {
            consume("(");

            while (this.currentToken != null && this.currentToken.Value.type != ")") {

                arguments.Add(parseCode());

                if (this.currentToken != null && this.currentToken.Value.type == ",") {
                    consume(",");
                }
            }
            consume(")");
        }

        return new F_CallNode(token, arguments);
    }

    public CodeNode parseCode() {

        List<AstNode> steps = new List<AstNode>();

        while (this.currentToken != null && this.currentToken.Value.type is "STEP" or "FUNCTION" or "PARAMETER") {

            if (this.currentToken.Value.type == "FUNCTION") {
                steps.Add(parseFunctionCall());
            } else
            if (this.currentToken.Value.type == "PARAMETER") {
                steps.Add(parseVariable());
            } else
            if (this.currentToken.Value.type == "STEP") {
                steps.Add(parseStep());
            }
        }

        return new CodeNode(steps);
    }

    public StepNode parseStep() {
        Token token = this.currentToken!.Value;
        consume("STEP");
        return new StepNode(token);
    }

    public VariableNode parseVariable() {
        Token token = this.currentToken!.Value;
        consume("PARAMETER");
        return new VariableNode(token);
    }

    public F_ParameterNode parseParameter() {
        Token token = this.currentToken!.Value;
        consume("PARAMETER");
        return new F_ParameterNode(token);
    }
}