namespace HerbertLang;


public class F_DefinitionNode : AstNode {
    public string name;

    public CodeNode code;
    
    public List<F_ParameterNode> parameters;

    public F_DefinitionNode(
        Token token,
        List<F_ParameterNode> parameters,
        CodeNode code
    ) {

        this.name = token.content;
        this.line = token.line;
        this.column = token.column;

        this.parameters = parameters;
        this.code = code;
    }

    public override string ToString() {
        return $"Definition({name})";
    }
}

public class F_ParameterNode : AstNode {
    public string name;

    public F_ParameterNode(Token token) {
        this.name = token.content;
        this.line = token.line;
        this.column = token.column;
    }

    public override string ToString() {
        return $"Parameter({name})";
    }
}
