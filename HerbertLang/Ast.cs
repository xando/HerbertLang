using System;
using System.Collections.Generic;


namespace HerberLang {

    public class Context {
        public Stack<StackFrame> stack;
        public Dictionary<string, FunctionDefinitionNode> definitions;

        public Context(Dictionary<string, FunctionDefinitionNode> definitions) {
            this.stack = new Stack<StackFrame>();
            this.stack.Push(new StackFrame());

            this.definitions = definitions;
        }
    }

    public class StackFrame {

        public Dictionary<string, INode> variables;
        public int durability = 1;

        public StackFrame(Dictionary<string, INode> variables, int durability) {
            this.variables = variables;
            this.durability = durability;
        }

        public StackFrame() {
            this.variables = new Dictionary<string, INode>();
        }

    }

    public abstract class INode {
        public int line = 0;
        public int column = 0;
        public virtual INode eval(Context context = null) {
            return this;
        }
    }

    public class ProgramNode : INode {

        public Dictionary<string, FunctionDefinitionNode> definitions;

        public NextNode next;

        public ProgramNode(Dictionary<string, FunctionDefinitionNode> definitions, NextNode next) {
            this.definitions = definitions;
            this.next = next;
        }

        public override INode eval(Context context = null) {
            var defaultContext = context ?? new Context(definitions: this.definitions);
            if (this.next == null) {
                return null;
            }

            return this.next.eval(defaultContext);
        }
    }

    public class StepNode : INode {
        public string step;
        public String step_;

        public StepNode(Token token) {
            this.step = token.content;
            this.line = token.line;
            this.column = token.column;

            if (token.content == "s") {
                this.step_ = Step.STEP_FORWARD;
            } else
               if (token.content == "r") {
                this.step_ = Step.TURN_RIGHT;
            } else
               if (token.content == "l") {
                this.step_ = Step.TURN_LEFT;
            } else {
                throw new Exception("Some generic");
            }

        }

        public override string ToString() {
            return $"Step({step})";
        }
    }

    public class FunctionCallNode : INode {

        public string name;
        public int durability;
        public bool durabilityOverwrite;
        public List<INode> arguments;
        
        public FunctionCallNode(Token token, List<INode> arguments, int durability, bool durabilityOverwrite) {
            this.name = token.content;
            this.arguments = arguments;
            this.line = token.line;
            this.column = token.column;

            this.durability = durability;
            this.durabilityOverwrite = durabilityOverwrite;
        }
        public override INode eval(Context context = null) {

            if (!context.definitions.ContainsKey(this.name)) {

                throw new LanguageError(
                    $"Function '{this.name}' is undefined.", this
                );

            }

            var definition = context.definitions[this.name];

            if (definition.parameters.Count > this.arguments.Count) {

                throw new LanguageError(
                    $"Funtion '{this.name}' has missing argument", this
                );

            }

            if (this.arguments.Count > definition.parameters.Count) {

                throw new LanguageError(
                    $"Funtion '{this.name}' has too many arguments", this
                );

            }

            var variables = new Dictionary<string, INode>();

            for (int i = 0; i < definition.parameters.Count; i++) {
                var functionParameter = definition.parameters[i];
                var name = (functionParameter as FunctionParameterNode).name;

                var argument = this.arguments[i].eval(context);

                if (argument is NumberNode) {
                    if ((argument as NumberNode).value < 1) {
                        return null;
                    }
                }
                variables.Add(name, argument);
            }

            int callDurability;

            if (this.durabilityOverwrite) {
                callDurability = this.durability;
            } else {

                callDurability = context.stack.Peek().durability + this.durability;
            }

            context.stack.Push(new StackFrame(variables, callDurability));

            if (context.stack.Count > 128) {
                return null;
                // throw new LanguageError("Call stack exceed max size 128", this);
            }

            INode code;

            if (callDurability < 1) {
                context.stack.Pop();
                return null;
            } else {
                code = definition.next.eval(context);
                context.stack.Pop();
            }

            return code;
        }

        public override string ToString() {
            return $"FunctionCall({name})";
        }
    }

    public class FunctionDefinitionNode : INode {
        public string name;

        public CodeNode code;
        public NextNode next;
        public List<INode> parameters;

        public FunctionDefinitionNode(Token token, List<INode> parameters, NextNode next) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
            this.next = next;
            this.parameters = parameters;
        }

        public override string ToString() {
            return $"Definition({name})";
        }
    }
    public class FunctionParameterNode : INode {
        public string name;

        public FunctionParameterNode(Token token) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
        }

    }

    public class CodeNode : INode {
        public List<INode> steps;

        public CodeNode(List<INode> steps) {
            this.steps = steps;
        }

        public override INode eval(Context context = null) {

            var steps = new List<INode>();
            foreach (var step in this.steps) {

                if (step is FunctionCallNode || step is VariableNode) {
                    var code = step.eval(context) as CodeNode;
                    steps.AddRange(code.steps);
                } else {
                    steps.Add(step.eval(context));
                }
            }
            return new CodeNode(steps);
        }

        public override string ToString() {

            var items = new List<string>();

            foreach (var step in steps) {
                items.Add(step.ToString());
            }

            return string.Join(", ", items.ToArray());
        }
    }

    public class NextNode : INode {
        public INode node;
        public string op;
        public NextNode next;

        public NextNode(string op, INode node, NextNode next) {
            this.op = op;
            this.node = node;
            this.next = next;
        }

        public NextNode(INode node) {
            this.node = node;
        }

        public override string ToString() {
            return $"Expresion({node} {op} {next})";
        }

        public override INode eval(Context context = null) {
            INode left;
            if (this.node is StepNode) {
                left = new CodeNode(new List<INode>() { this.node.eval(context) });
            } else {
                left = this.node.eval(context);
            }

            if (this.next == null) {
                return left;
            }

            var right = this.next.eval(context);

            if (right == null) {
                return left;
            }
            if (left == null) {
                return right;
            }

            if (left.GetType() != right.GetType()) {
                throw new LanguageError("Bad operation", right.line, right.column);
            }

            if (right is NumberNode) {
                int leftValue = ((NumberNode)left).value;
                int rightValue = ((NumberNode)right).value;

                if (op == "+") {
                    return new NumberNode(leftValue + rightValue);
                }

                if (op == "-") {
                    return new NumberNode(leftValue - rightValue);
                }
            }

            if (right is CodeNode) {
                (left as CodeNode).steps.AddRange((right as CodeNode).steps);
                return left;
            }

            throw new NotSupportedException();
        }
    }

    public class NumberNode : INode {
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

    public class VariableNode : INode {
        public string name;

        public VariableNode(Token token) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
        }

        public override INode eval(Context context = null) {
            var variables = context.stack.Peek().variables;
            if (variables.ContainsKey(this.name)) {
                return variables[this.name].eval(context);
            }
            throw new LanguageError($"Variable '{this.name}' is undefined", this);
        }
    }
}