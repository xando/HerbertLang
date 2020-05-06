using System;
using System.Collections.Generic;


namespace HerberLanguage {

    public class Context {
        public Stack<StackFrame> stack;
        public Dictionary<string, FunctionDefinition> functionDefinitions;

        public Context(Dictionary<string, FunctionDefinition> definitions) {
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

    public class Program : INode {

        public Dictionary<string, FunctionDefinition> definitions;

        public Next next;

        public Program(Dictionary<string, FunctionDefinition> definitions, Next next) {
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

    public class Step : INode {
        public string step;
        public ExecutionStep step_;

        public Step(Token token) {
            this.step = token.content;
            this.line = token.line;
            this.column = token.column;

            if (token.content == "s")
            {
                this.step_ = ExecutionStep.STEP_FORWARD;
            }
            else
               if (token.content == "r")
            {
                this.step_ = ExecutionStep.TURN_RIGHT;
            }
            else
               if (token.content == "l")
            {
                this.step_ = ExecutionStep.TURN_LEFT;
            }
            else {
                throw new Exception("Some generic");
            }

        }

        public override string ToString() {
            return string.Format("Step({0})", step);
        }
    }

    public class FunctionCall : INode {

        public string name;
        public int durability;
        public bool durabilitySet;
        public List<INode> arguments;

        public FunctionCall(string name, List<INode> codeArguments, int line, int column) {
            this.name = name;
            this.arguments = codeArguments;
            this.line = line;
            this.column = column;
        }
        public FunctionCall(string name, List<INode> arguments) {
            this.name = name;
            this.arguments = arguments;
            this.line = 0;
            this.column = 0;
        }
        public FunctionCall(string name) {
            this.name = name;
            this.arguments = new List<INode>();
            this.line = 0;
            this.column = 0;
        }
        public FunctionCall(Token token, List<INode> arguments) {
            this.name = token.content;
            this.arguments = arguments;
            this.line = token.line;
            this.column = token.column;
        }
        public FunctionCall(Token token, List<INode> arguments, int durability, bool durabilitySet) {
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
                var name = (functionParameter as FunctionParameter).name;

                var argument = this.arguments[i].eval(context);

                if (argument is Number) {
                    if ((argument as Number).value < 1) {
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

    public class FunctionDefinition : INode {
        public string name;

        public Code code;
        public Next next;
        public List<INode> parameters;

        public FunctionDefinition(string name, List<INode> parameters, Code code) {
            this.name = name;
            this.code = code;
            this.parameters = parameters;
        }
        public FunctionDefinition(string name, List<INode> parameters, Next next) {
            this.name = name;
            this.parameters = parameters;
            this.next = next;
        }

        public FunctionDefinition(Token token, List<INode> parameters, Code code) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
            this.code = code;
            this.parameters = parameters;
        }

        public FunctionDefinition(Token token, List<INode> parameters, Next next) {
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
    public class FunctionParameter : INode {
        public string name;

        public FunctionParameter(Token token) {
            this.name = token.content;
            this.line = token.line;
            this.column = token.column;
        }

        public FunctionParameter(string name) {
            this.name = name;
        }

    }

    public class Code : INode {
        public List<INode> steps;

        public Code(List<INode> steps) {
            this.steps = steps;
        }

        public Code() {
            this.steps = new List<INode>();
        }

        public override INode eval(Context context = null) {

            var steps = new List<INode>();
            foreach (var step in this.steps) {

                if (step is FunctionCall || step is Variable) {
                    var code = step.eval(context) as Code;
                    steps.AddRange(code.steps);
                } else {
                    steps.Add(step.eval(context));
                }
            }
            return new Code(steps);
        }

        public override string ToString() {

            var items = new List<string>();

            foreach (var step in steps) {
                items.Add(step.ToString());
            }

            return string.Join(", ", items.ToArray());
        }
    }

    public class Next : INode {
        public INode node;
        public string op;
        public Next next;

        public Next(string op, INode node, Next next) {
            this.op = op;
            this.node = node;
            this.next = next;
        }

        public Next(INode node, Next next) {
            this.node = node;
            this.next = next;
        }

        public Next() {
        }

        public Next(INode node) {
            this.node = node;
        }

        public override string ToString() {
            return string.Format("Expresion({left} {op} {right})", node, op, next);
        }

        public override INode eval(Context context = null) {
            INode left;
            if (this.node is Step) {
                left = new Code(new List<INode>(){ this.node.eval(context) });
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

            if(left.GetType() != right.GetType()) {
                throw new LanguageError("Bad operation", right.line, right.column);
            }

            if (right is Number) {
                int leftValue = ((Number)left).value;
                int rightValue = ((Number)right).value;

                if (op == "+") {
                    return new Number(leftValue + rightValue);
                }

                if (op == "-") {
                    return new Number(leftValue - rightValue);
                }
            }

            if (right is Code) {
                (left as Code).steps.AddRange((right as Code).steps);
                return left;
            }

            throw new NotSupportedException();
        }
    }

    public class Number : INode {
        public int value;

        public Number(int value) {
            this.value = value;
        }

        public Number(Token token) {
            this.value = Int32.Parse(token.content);
            this.line = token.line;
            this.column = token.column;
        }

        public override string ToString() {
            return string.Format("Number({0})", value);
        }
    }

    public class Variable : INode {
        public string name;

        public Variable(string name) {
            this.name = name;
        }

        public Variable(Token token) {
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