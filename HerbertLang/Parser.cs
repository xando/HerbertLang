using System;
using System.Collections.Generic;


namespace HerberLang {
    public class Parser {

        public static INode parse(List<Token> tokens) {
            var parser = new Parser(tokens);
            return parser.parseProgram();
        }

        List<Token> tokens;
        int position;

        public Parser(List<Token> tokens) {
            this.tokens = tokens;
            this.position = 0;
        }

        Token? peek(int ahead = 0) {
            if (position + ahead < tokens.Count) {
                return tokens[position + ahead];
            }
            return null;
        }

        Token? last() {
            if (position - 1 > 0) {
                return tokens[position - 1];
            }
            return null;
        }

        bool look(string type, string until) {
            for (int i = 1; i < tokens.Count; i++) {
                if (!peek(i).HasValue) {
                    return false;
                }
                if (peek(i).Value.type == until) {
                    return false;
                }
                if (peek(i).Value.type == type) {
                    return true;
                }
            }
            return false;
        }

        void consume(string type) {
            if (position >= tokens.Count) {
                var prevToken = tokens[position - 1];
                var msg = string.Format("Expected '{0}'", type);

                throw new LanguageError(msg, prevToken.line, prevToken.column);
            }
            var token = tokens[position];
            if (token.type != type) {
                var msg = string.Format("Expected '{0}' got '{1}'", type, token.type);
                throw new LanguageError(msg, token.line, token.column);
            }
            position++;
        }

        public FunctionDefinitionNode parseFunctionDefinition() {
            var parameters = new List<INode>();
            var parametersNames = new List<string>();

            Token? t = peek();

            var token = t.Value;

            consume("FUNCTION");

            t = peek();
            if (t.HasValue && t.Value.type == "(") {

                consume("(");

                while (peek().HasValue && peek().Value.type == "PARAMETER") {
                    var parameterToken = peek().Value;
                    var parameter = new FunctionParameterNode(parameterToken);

                    if (parametersNames.Contains(parameter.name)) {
                        throw new LanguageError("Duplicate parameter", parameter);
                    }

                    parameters.Add(parameter);
                    parametersNames.Add(parameter.name);

                    consume("PARAMETER");

                    if (peek().HasValue && peek().Value.type == "," && peek(1).Value.type == "PARAMETER") {
                        consume(",");
                    } else {
                        break;
                    }
                }
                consume(")");
            }
            consume(":");
            var next = parseNext();
            var definition = new FunctionDefinitionNode(token, parameters, next);
            return definition;
        }

        public ProgramNode parseProgram() {

            Token? t = peek();

            var definitions = new Dictionary<string, FunctionDefinitionNode>();
            NextNode start = null;

            do {
                while (t.HasValue && t.Value.type == "NEW_LINE") {
                    consume("NEW_LINE");
                    t = peek();
                }

                bool isDefinitionLine = look(":", until: "\n");

                if (isDefinitionLine) {
                    var definition = parseFunctionDefinition();
                    if (definitions.ContainsKey(definition.name)) {
                        var msg = string.Format("Function '{0}' already defined.", definition.name);
                        throw new LanguageError(msg, definition);
                    }
                    definitions[t.Value.content] = definition;
                } else {
                    if (start == null) {
                        start = parseNext();
                    } else {
                        t = peek();
                        if (t.HasValue) {
                            var last = start;
                            while (last.next != null) {
                                last = last.next;
                            }
                            last.next = parseNext();
                        }
                    }
                }

                t = peek();

            } while (t.HasValue && t.Value.type == "NEW_LINE");

            return new ProgramNode(definitions, start);
        }

        public FunctionCallNode parseFunctionCall() {
            var arguments = new List<INode>();
            bool durabilitySet = false;
            var durability = 1;

            Token? t = peek();

            var token = t.Value;

            consume("FUNCTION");

            var functionToken = t.Value;

            t = peek();

            if (t.HasValue && (
                t.Value.type == "=" ||
                t.Value.type == "+" ||
                t.Value.type == "-")) {
                if (t.Value.type == "=") {
                    durabilitySet = true;
                    consume("=");
                    var number = parseNumer();

                    durability = number.value;
                }
                if (t.Value.type == "+") {
                    consume("+");
                    var number = parseNumer();
                    durability = number.value;
                }
                if (t.Value.type == "-") {
                    consume("-");
                    var number = parseNumer();
                    durability = -number.value;
                }
            }

            if (t.HasValue && t.Value.type == "(") {
                consume("(");

                while (peek().HasValue && peek().Value.type != ")") {

                    arguments.Add(parseNext());

                    if (peek().HasValue && peek().Value.type == ",") {
                        consume(",");
                    } else {
                        break;
                    }
                }
                consume(")");
            }

            return new FunctionCallNode(functionToken, arguments,
            durability, durabilitySet);
        }

        public CodeNode parseCode() {

            List<INode> steps = new List<INode>();

            Token? t = peek();

            while (t.HasValue && (
                t.Value.type == "STEP" ||
                t.Value.type == "FUNCTION" ||
                t.Value.type == "PARAMETER")) {

                if (t.Value.type == "FUNCTION") {
                    steps.Add(parseFunctionCall());

                } else if (t.Value.type == "PARAMETER") {
                    steps.Add(new VariableNode(t.Value));
                    consume("PARAMETER");

                } else if (t.Value.type == "STEP") {
                    steps.Add(new StepNode(t.Value));
                    consume("STEP");
                }

                t = peek();
            }

            return new CodeNode(steps);
        }

        public NextNode parseNext() {
            Token? t = peek();
            INode node = null;

            if (!t.HasValue) {
                t = last();
                string msg;
                if (t != null) {
                    msg = string.Format("Some code expected after {0}", t.Value.content);
                    throw new LanguageError(msg, t.Value.line, t.Value.column);
                }
                msg = string.Format("Some code expected.");
                throw new LanguageError(msg, 1, 1);
            }

            if (t.Value.type == "STEP") {
                node = new StepNode(t.Value);
                consume("STEP");
            } else
            if (t.Value.type == "PARAMETER") {
                node = new VariableNode(t.Value);
                consume("PARAMETER");
            } else
            if (t.Value.type == "NUMBER") {
                node = new NumberNode(t.Value);
                consume("NUMBER");
            } else
            if (t.Value.type == "FUNCTION") {
                node = parseFunctionCall();
            } else {
                string msg;
                if (t.Value.content == "\n") {
                    msg = string.Format("Unexpected new line.", t.Value.content);
                } else {
                    msg = string.Format("Unexpected character '{0}'.", t.Value.content);
                }

                throw new LanguageError(msg, t.Value.line, t.Value.column);
            }

            t = peek();

            if (t.HasValue) {
                string op = null;

                if (t.Value.type == "+") {
                    op = "+";
                    consume("+");

                    if (!peek().HasValue) {
                        var msg = string.Format("Some code expected after {0}", t.Value.content);
                        throw new LanguageError(msg, t.Value.line, t.Value.column);
                    }

                    t = peek();
                }

                if (t.Value.type == "-") {
                    op = "-";
                    consume("-");

                    if (!peek().HasValue) {
                        var msg = string.Format("Some code expected after {0}", t.Value.content);
                        throw new LanguageError(msg, t.Value.line, t.Value.column);
                    }

                    t = peek();
                }

                if (t.HasValue && (t.Value.type == "STEP" ||
                                   t.Value.type == "PARAMETER" ||
                                   t.Value.type == "NUMBER" ||
                                   t.Value.type == "FUNCTION")) {
                    return new NextNode(op, node, parseNext());
                }
                if (op != null) {
                    var msg = string.Format("Unexpected character '{0}'.", op);
                    throw new LanguageError(msg, t.Value.line, t.Value.column + 1);
                }
            }
            return new NextNode(node);
        }

        public NumberNode parseNumer() {
            Token? t = peek();

            var sign = 1;

            while (t?.type == "-") {
                sign *= -1;
                consume("-");
                t = peek();
            }
            var numer = Int32.Parse(t?.content);

            consume("NUMBER");

            return new NumberNode(numer * sign);

        }
    }
}
