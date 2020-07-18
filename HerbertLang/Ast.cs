using System;
using System.Collections.Generic;


namespace HerberLang {

    public class Context {
        public Stack<StackFrame> stack;
        public Dictionary<string, FunctionDefinitionNode> functionDefinitions;

        public Context(Dictionary<string, FunctionDefinitionNode> definitions) {
            this.stack = new Stack<StackFrame>();
            this.stack.Push(new StackFrame());

            this.functionDefinitions = definitions;
        }
    }

    public class StackFrame {

        public Dictionary<string, INode> variables;
        public int durability = 1;

        public StackFrame(Dictionary<string, INode> variables, int durability) {
            this.variables = variables;
            this.durability = durability;
        }

        public StackFrame(int durability) {
            this.variables = new Dictionary<string, INode>();
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
        public Step step_;

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
            return string.Format("Step({0})", step);
        }
    }

    public class FunctionCallNode : INode {

        public string name;
        public int durability;
        public bool durabilitySet;
        public List<INode> arguments;

        public FunctionCallNode(string name, List<INode> codeArguments, int line, int column) {
            this.name = name;
            this.arguments = codeArguments;
            this.line = line;
            this.column = column;
        }
        public FunctionCallNode(string name, List<INode> arguments) {
            this.name = name;
            this.arguments = arguments;
            this.line = 0;
            this.column = 0;
        }
        public FunctionCallNode(string name) {
            this.name = name;
            this.arguments = new List<INode>();
            this.line = 0;
            this.column = 0;
        }
        public FunctionCallNode(Token token, List<INode> arguments) {
            this.name = token.content;
            this.arguments = arguments;
            this.line = token.line;
            this.column = token.column;
        }
        public FunctionCallNode(Token token, List<INode> arguments, int durability, bool durabilitySet) {
            this.name = token.content;
            this.arguments = arguments;
            this.line = token.line;
            this.column = token.column;

            this.durability = durability;
            this.durabilitySet = durabilitySet;
        }
        public override INode eval(Context context = null) {

            if (!context.functionDefinitions.ContainsKey(this.name)) {
                var msg = string.Format("Function '{0}' is undefined", this.name);
                throw new LanguageError(msg, this);
            }

            var definition = context.functionDefinitions[this.name];

            if (definition.parameters.Count > this.arguments.Count) {
                var msg = string.Format("Funtion '{0}' has missing argument", this.name);
                throw new LanguageError(msg, this);
            }

            if (this.arguments.Count > definition.parameters.Count) {
                var msg = string.Format("Funtion '{0}' has too many arguments", this.name);
                throw new LanguageError(msg, this);
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

            if (this.durabilitySet) {
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
            return string.Format("FunctionCall({0})", name);
        }
    }

    public class FunctionDefinitionNode : INode {
        public string name;

        public CodeNode code;
        public NextNode next;
        public List<INode> parameters;

        public FunctionDefinitionNode(string name, List<INode> parameters, CodeNode code) {
            this.name = name;
            this.code = code;
            this.parameters = parameters;
        }
        public FunctionDefinitionNode(string name, List<INode> parameters, NextNode next) {
            this.name = name;
            this.parameters = parameters;
            this.next = next;
        }

        public FunctionDefinitionNode(Token token, List<INode> parameters, CodeNode code) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
            this.code = code;
            this.parameters = parameters;
        }

        public FunctionDefinitionNode(Token token, List<INode> parameters, NextNode next) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
            this.next = next;
            this.parameters = parameters;
        }

        public override string ToString() {
            return string.Format("Definition({0})", name);
        }
    }
    public class FunctionParameterNode : INode {
        public string name;

        public FunctionParameterNode(Token token) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
        }

        public FunctionParameterNode(string name) {
            this.name = name;
        }

    }

    public class CodeNode : INode {
        public List<INode> steps;

        public CodeNode(List<INode> steps) {
            this.steps = steps;
        }

        public CodeNode() {
            this.steps = new List<INode>();
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

        public NextNode(INode node, NextNode next) {
            this.node = node;
            this.next = next;
        }

        public NextNode() {
        }

        public NextNode(INode node) {
            this.node = node;
        }

        public override string ToString() {
            return string.Format("Expresion({left} {op} {right})", node, op, next);
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
            return string.Format("Number({0})", value);
        }
    }

    public class VariableNode : INode {
        public string name;

        public VariableNode(string name) {
            this.name = name;
        }

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
            var msg = string.Format("Variable '{0}' is undefined", this.name);
            throw new LanguageError(msg, this);
        }
    }
}