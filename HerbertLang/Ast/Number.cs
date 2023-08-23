namespace HerbertLang;


public class NumberNode : AstNode {
    public int value;

    public NumberNode(int value) {
        this.value = value;
    }

    public NumberNode(Token token) {
        this.value = Int32.Parse(token.content);
        this.line = token.line;
        this.column = token.column;
    }

    public override string ToString() {
        return $"Number({value})";
    }
}
