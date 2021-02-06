using Xunit;

using HerberLang;
using System.Collections.Generic;

namespace HerbertLang.Tests {
    public class TestInterpter {

        [Fact]
        public void Test_ToString() {
            Assert.Equal("sss", Interpreter.evalToString("sss"));
        }

        [Fact]
        public void Test_ToString_Error() {
            Assert.Equal("1:1 Function 'f' is undefined", Interpreter.evalToString("fss"));
        }


        [Fact]
        public void Test_ToCode() {
            Assert.Equal(new List<Step>(){
                Step.STEP_FORWARD,
                Step.STEP_FORWARD,
                Step.STEP_FORWARD,
            }, Interpreter.evalToCode("sss"));
        }

        [Fact]
        public void Test_ToCode_Exception() {

            LanguageError ex = Assert.Throws<LanguageError>(() => Interpreter.evalToCode("f"));

            Assert.Equal(1, ex.line);
            Assert.Equal(1, ex.column);
        }

    }

    public class TestInterpterToString {

        [Fact]
        public void EvalProgram_1() {

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
        public void EvalProgram_2() {
            var code = @"
f:rr
fsss
            ";
            Assert.Equal(
                "rrsss",
                Interpreter.evalToString(code)
            );

        }

        [Fact]
        public void EvalProgram_21() {
            var h = @"
f(A):A
f(ss)";

            Assert.Equal("ss",
            Interpreter.evalToString(h)
            );
        }


        [Fact]
        public void EvalProgram_22() {
            var h = @"
f(A, B):AB
f(ss, rr)";

            Assert.Equal("ssrr", Interpreter.evalToString(h));
        }


        [Fact]
        public void EvalProgram_3() {

            var h = @"
z:ll
f:rrz
fsss";

            Assert.Equal("rrllsss", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_4() {

            var h = @"
f(A):A
f(ss)rr";

            Assert.Equal("ssrr", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_5() {

            var h = @"
f(A,B):AB
f(ss,ll)rr";

            Assert.Equal("ssllrr", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_6() {

            var h = @"
            z:sss
            f(A,B):AzzzB
            f(ss,ll)rr";

            Assert.Equal("sssssssssssllrr", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_7() {
            var h = @"
z:sss
f(A):lA
f(z)rr";

            Assert.Equal("lsssrr", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_8() {
            var h = @"
z:sss
f(A):lA
f(ss)r";

            Assert.Equal("lssr", Interpreter.evalToString(h));
        }


        [Fact]
        public void EvalProgram_10() {
            var h = @"
f:f
f";

            Assert.Equal("", Interpreter.evalToString(h));
        }
        [Fact]
        public void EvalProgram_11() {
            var h = @"
f(A):f(A)s
f(0)";

            Assert.Equal("", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_12() {
            var h = @"
f(A):sf(A-1)
f(2)";

            Assert.Equal("ss", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_13() {
            var h = @"
f(A):f(A-1)s
f(2)";

            Assert.Equal("ss", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_14() {
            var h = @"
f(A):ss
f(2)";

            Assert.Equal("ss", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_15() {
            var h = @"

sss

";

            Assert.Equal("sss", Interpreter.evalToString(h));
        }

        [Fact]
        public void EvalProgram_16() {
            var h = @"
f:f";

            Assert.Equal("", Interpreter.evalToString(h));
        }

    }
}