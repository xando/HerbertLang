namespace HerbertLang;

public abstract class AstNode {
    public int line = 0;
    public int column = 0;
    public virtual AstNode eval(Context context) {
        return this;
    }
    public virtual AstNode eval() {
        return this;
    }
}
