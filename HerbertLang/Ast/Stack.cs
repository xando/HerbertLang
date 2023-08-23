namespace HerbertLang;


public class StackFrame {

    public Dictionary<string, CodeNode> variables;

    public StackFrame(Dictionary<string, CodeNode> variables) {
        this.variables = variables;
    }

    public StackFrame() {
        this.variables = new Dictionary<string, CodeNode>();
    }

}
