namespace HerbertLang;


public class StackFrame {

    public Dictionary<string, AstNode> variables;

    public StackFrame(Dictionary<string, AstNode> variables) {
        this.variables = variables;
    }

    public StackFrame() {
        this.variables = new Dictionary<string, AstNode>();
    }

}
