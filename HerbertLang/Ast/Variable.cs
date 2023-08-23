namespace HerbertLang;


public class VariableNode : AstNode {
    public string name;

    public VariableNode(Token token) {
        this.name = token.content;
        this.line = token.line;
        this.column = token.column;
    }

    public override VariableNode eval(Context? context = null) {
        return this;
        // var variables = context.stack.Peek().variables;
        // if (variables.ContainsKey(this.name)) {
        //     return new StepCollectionNode(new List<StepNode>());
        //     // return variables[this.name].eval(context);
        // }
        // throw new LanguageError($"Variable '{this.name}' is undefined", this);
    }
}