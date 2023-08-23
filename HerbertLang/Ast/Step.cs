namespace HerbertLang;


public class StepNode : AstNode {
    public string step;

    public StepNode(Token token) {
        this.step = token.content;
        this.line = token.line;
        this.column = token.column;
    }

    public override StepNode eval(Context context) {
        return this;
    }

    public override string ToString() {
        return $"Step({step})";
    }
}
