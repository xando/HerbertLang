namespace HerbertLang;


public class F_CallNode : AstNode {

    public string name;

    public List<CodeNode> arguments;

    public F_CallNode(Token token, List<CodeNode> arguments) {
        this.name = token.content;
        this.line = token.line;
        this.column = token.column;

        this.arguments = arguments;

    }
    public override CodeNode eval(Context context) {

        if (!context.f_definitions.ContainsKey(this.name)) {
            // NOTE: Function was not defined before.
            throw new LanguageError($"Function '{this.name}' is undefined.", this);
        }

        var definition = context.f_definitions[this.name];

        if (definition.parameters.Count > this.arguments.Count) {
            // NOTE: Number of arguments is less than number of parameters.
            //       UNSUPPORTED.
            throw new LanguageError($"Function '{this.name}' has missing argument", this);
        }

        if (this.arguments.Count > definition.parameters.Count) {
            // NOTE: Number of arguments is more than number of parameters.
            //       UNSUPPORTED.
            throw new LanguageError($"Function '{this.name}' has too many arguments", this);
        }

        // var variables = new Dictionary<string, AstNode>();

        // for (int i = 0; i < definition.parameters.Count; i++) {
        //     var functionParameter = definition.parameters[i];
        //     var name = (functionParameter as F_ParameterNode).name;

        //     var argument = this.arguments[i].eval(context);
        //     variables.Add(name, argument);
        // }

        // context.stack.Push(new StackFrame(variables));

        if (context.stack.Count > 128) {
            throw new LanguageError("Call stack exceed max size 128", this);
        }

        var codeNode = new CodeNode(definition.code.steps);
        return codeNode.eval(context);
    }

    public override string ToString() {
        return $"FunctionCall({name})";
    }
}
