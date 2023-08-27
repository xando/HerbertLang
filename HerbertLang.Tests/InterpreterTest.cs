#pragma warning disable xUnit2013

using Xunit;

namespace HerbertLang.Tests;


public class TestInterpreter {

    [Fact]
    public void TestEmpty() {
        Assert.Equal("", Interpreter.evalToString("").code);
    }


    [Fact]
    public void TestCode() {
        Assert.Equal("srl", Interpreter.evalToString("srl").code);
    }

    [Fact]
    public void TestCodeMultipleLinies() {

        var input = @"
rr
lsr
";
        Assert.Equal(
            "rrlsr",
            Interpreter.evalToString(input).code
        );
    }

    [Fact]
    public void TestFunctionDefintion() {
        var input = @"
#f:s
rrrf
lllf
";

        Assert.Equal("rrrsllls", Interpreter.evalToString(input).code);
    }

[Fact]
    public void TestMultipleFunctions() {
        var input = @"
#z:s
#f:sz
rrrf
lllf
";

        Assert.Equal("rrrsslllss", Interpreter.evalToString(input).code);
    }

    [Fact]
    public void TestUndefinedError() {
        var input = "fss";

        Assert.Equal("1:1 Function 'f' is undefined.", Interpreter.evalToString(input).error);
    }

    [Fact]
    public void TestFunctionWithParameters() {
        var input = @"
#f(A):A
f(s)
";
        Assert.Equal("s", Interpreter.evalToString(input).code);
    }

}