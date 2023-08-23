using HerbertLang;


public class Context {
    
    // Stack - locals available at the given level of the execution
    public readonly Stack<StackFrame> stack;

    // Function Definitions - are global, We should be access them and any level
    // of execution stack.
    public readonly Dictionary<string, F_DefinitionNode> f_definitions;

    public Context(Dictionary<string, F_DefinitionNode> f_definitions) {
        this.stack = new Stack<StackFrame>();
        this.stack.Push(new StackFrame());

        this.f_definitions = f_definitions;
    }
}
