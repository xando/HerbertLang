namespace HerbertLang;


public class VariableNode : AstNode {
    public string name;

    public VariableNode(Token token) {
        this.name = token.content;
        this.line = token.line;
        this.column = token.column;
    }

    public override CodeNode eval(Context context) {
        var variables = context.stack.Peek().variables;
        
        if(variables.ContainsKey(this.name)) {
            return variables[this.name].eval(context);
        }

        throw new LanguageError($"Variable '{this.name}' is undefined", this);
    }
}