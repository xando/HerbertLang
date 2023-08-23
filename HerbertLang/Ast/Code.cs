namespace HerbertLang;


public class CodeNode : AstNode {

    public List<AstNode> steps;

    public CodeNode(List<AstNode> steps) {
        this.steps = steps;
    }

    public CodeNode() {
        this.steps = new List<AstNode>();
    }

    public void extend(CodeNode codeNode) {
        this.steps.AddRange(codeNode.steps);
    }

    public override CodeNode eval(Context context) {

        var codeNode = new CodeNode();

        foreach (var step in this.steps) {
           
            if (step is StepNode) {
                codeNode.steps.Add(
                    ((StepNode)step).eval(context)
                );
            } else
            if (step is F_CallNode) {
                codeNode.steps.AddRange(
                    ((F_CallNode)step).eval(context).steps
                );
            } 
            // else if (step is VariableNode) {
            //     steps.AddRange(
            //         ((VariableNode)step).eval(context).steps
            //     );
            // } 

            else {
                throw new NotSupportedException($"{step.ToString()} not supported");
            }
        }
        return codeNode;
    }
}
