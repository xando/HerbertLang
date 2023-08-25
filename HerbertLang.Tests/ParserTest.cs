#pragma warning disable xUnit2013

using Xunit;

namespace HerbertLang.Tests;


public class ParserTest {

    [Fact]
    public void Step() {
        var tokens = Lexer.tokenize("s");
        var parser = new Parser(tokens);

        StepNode step = parser.parseStep();

        Assert.Equal(1, step.column);
        Assert.Equal(1, step.line);
        Assert.Equal("s", step.step);
    }

    [Fact]
    public void Variable() {
        var tokens = Lexer.tokenize("A");
        var parser = new Parser(tokens);

        VariableNode variable = parser.parseVariable();

        Assert.Equal(1, variable.column);
        Assert.Equal(1, variable.line);
        Assert.Equal("A", variable.name);
    }

    [Fact]
    public void FunctionCall() {

        var tokens = Lexer.tokenize("z");
        var parser = new Parser(tokens);

        F_CallNode functionCall = parser.parseFunctionCall();

        Assert.Equal("z", functionCall.name);
        Assert.Equal(1, functionCall.line);
        Assert.Equal(1, functionCall.column);

        Assert.Equal(0, functionCall.arguments.Count);
    }

    [Fact]
    public void FunctionCall_With_Arguments() {

        var tokens = Lexer.tokenize("z(s, B, z)");
        var parser = new Parser(tokens);

        F_CallNode functionCall = parser.parseFunctionCall();

        Assert.Equal("z", functionCall.name);
        Assert.Equal(1, functionCall.line);
        Assert.Equal(1, functionCall.column);

        Assert.Equal(3, functionCall.arguments.Count);

        Assert.IsType<CodeNode>(functionCall.arguments[0]);
        Assert.IsType<CodeNode>(functionCall.arguments[1]);
        Assert.IsType<CodeNode>(functionCall.arguments[2]);

        Assert.IsType<StepNode>(functionCall.arguments[0].steps[0]);
        Assert.IsType<VariableNode>(functionCall.arguments[1].steps[0]);
        Assert.IsType<F_CallNode>(functionCall.arguments[2].steps[0]);
    }

    [Fact]
    public void Code() {
        var tokens = Lexer.tokenize("sss");
        var parser = new Parser(tokens);

        CodeNode code = parser.parseCode();

        Assert.Equal(3, code.steps.Count);

        Assert.Equal(1, code.steps[0].column);
        Assert.Equal(2, code.steps[1].column);
        Assert.Equal(3, code.steps[2].column);

        Assert.Equal(1, code.steps[0].line);
        Assert.Equal(1, code.steps[1].line);
        Assert.Equal(1, code.steps[2].line);
    }

    [Fact]
    public void Code_More() {
        var tokens = Lexer.tokenize("srlAf");
        var parser = new Parser(tokens);

        CodeNode code = parser.parseCode();

        Assert.Equal(5, code.steps.Count);

        Assert.Equal(1, code.steps[0].line);
        Assert.Equal(1, code.steps[1].line);
        Assert.Equal(1, code.steps[2].line);
        Assert.Equal(1, code.steps[3].line);
        Assert.Equal(1, code.steps[4].line);

        Assert.Equal(1, code.steps[0].column);
        Assert.Equal(2, code.steps[1].column);
        Assert.Equal(3, code.steps[2].column);
        Assert.Equal(4, code.steps[3].column);
        Assert.Equal(5, code.steps[4].column);

        Assert.IsType<StepNode>(code.steps[0]);
        Assert.IsType<StepNode>(code.steps[1]);
        Assert.IsType<StepNode>(code.steps[2]);

        Assert.IsType<VariableNode>(code.steps[3]);
        Assert.IsType<F_CallNode>(code.steps[4]);
    }

    [Fact]
    public void FunctionDefinition() {

        var tokens = Lexer.tokenize("#f:sss");
        var parser = new Parser(tokens);

        F_DefinitionNode definition = parser.parseFunctionDefinition();

        Assert.Equal("f", definition.name);
        Assert.Equal(1, definition.line);
        Assert.Equal(2, definition.column);

        Assert.Equal(0, definition.parameters.Count);
        Assert.Equal(3, definition.code.steps.Count);
    }

    [Fact]
    public void FunctionDefinitionMultiple() {
        var tokens = Lexer.tokenize(@"
#f:sss
#z:sss
");
        var parser = new Parser(tokens);

        var programNode = parser.parseProgram();

        Assert.Equal(programNode.f_definitions.Count, 2);
        Assert.True(programNode.f_definitions.ContainsKey("f"));
        Assert.True(programNode.f_definitions.ContainsKey("z"));
    }

    [Fact]
    public void FunctionDefinitionWithArguments() {
        var tokens = Lexer.tokenize("#f(A,B):ssAB");
        var parser = new Parser(tokens);

        F_DefinitionNode definition = parser.parseFunctionDefinition();

        Assert.Equal("f", definition.name);
        Assert.Equal(2, definition.column);
        Assert.Equal(1, definition.line);

        Assert.Equal(2, definition.parameters.Count);
        Assert.Equal("A", definition.parameters[0].name);
        Assert.Equal("B", definition.parameters[1].name);
    }

    [Fact]
    public void FunctionDefinitionWithArgumentsDuplicated() {
        var tokens = Lexer.tokenize("#f( A, A ):ssAA");
        var parser = new Parser(tokens);

        Assert.Throws<LanguageError>(() => parser.parseFunctionDefinition());
    }

    [Fact]
    public void ProgramJustCode() {
        var tokens = Lexer.tokenize(@"
sss

lll
");
        var parser = new Parser(tokens);

        var programNode = parser.parseProgram();

        var code = programNode.code;

        Assert.Equal(6, code.steps.Count);

        Assert.Equal(2, code.steps[0].line);
        Assert.Equal(2, code.steps[1].line);
        Assert.Equal(2, code.steps[2].line);

        Assert.Equal(4, code.steps[3].line);
        Assert.Equal(4, code.steps[4].line);
        Assert.Equal(4, code.steps[4].line);

    }

    [Fact]
    public void Program() {
        var tokens = Lexer.tokenize(@"
#f(A):A
f(s)lllss
");
        var parser = new Parser(tokens);
        var programNode = parser.parseProgram();

        Assert.IsType<ProgramNode>(programNode);

        Assert.Equal(1, programNode.f_definitions.Count);
        Assert.Equal(6, programNode.code.steps.Count);
    }

}