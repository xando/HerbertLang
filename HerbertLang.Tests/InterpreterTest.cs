#pragma warning disable xUnit2013

using Xunit;

namespace HerbertLang.Tests;


public class TestInterpreter {

    [Fact]
    public void TestEmpty() {
        Assert.Equal("", Interpreter.evalToString(""));
    }


    [Fact]
    public void TestCode() {
        Assert.Equal("srl", Interpreter.evalToString("srl"));
    }

    [Fact]
    public void TestCodeMultipleLinies() {

        var code = @"
rr
lsr
";
        Assert.Equal(
            "rrlsr",
            Interpreter.evalToString(code)
        );
    }

    [Fact]
    public void TestFunctionDefintion() {
        var code = @"
f:s
rrrf
lllf
";

        Assert.Equal("rrrsllls", Interpreter.evalToString(code));
    }

[Fact]
    public void TestMultipleFunctions() {
        var code = @"
z:s
f:sz
rrrf
lllf
";

        Assert.Equal("rrrsslllss", Interpreter.evalToString(code));
    }

    [Fact]
    public void TestUndefinedError() {
        var code = "fss";

        Assert.Equal("1:1 Function 'f' is undefined.", Interpreter.evalToString(code));
    }

}

