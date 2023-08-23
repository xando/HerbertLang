namespace HerbertLang;


public class ProgramNode : AstNode {

    public Dictionary<string, F_DefinitionNode> f_definitions;
    public CodeNode code;

    public ProgramNode(
        Dictionary<string, F_DefinitionNode> definitions, CodeNode code) {
        this.f_definitions = definitions;
        this.code = code;
    }

    public override AstNode eval() {
        var context = new Context(this.f_definitions);
        return this.code.eval(context);
    }
}
