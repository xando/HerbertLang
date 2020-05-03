using Xunit;

using HerberLanguage;
using System.Collections.Generic;

namespace HerbertLang.Tests {
    public class TestInterpter {

        [Fact]
        public void Test_ToString() {
            Assert.Equal(Interpreter.toString("sss"), "sss");
        }

        [Fact]
        public void Test_ToString_Error() {
            Assert.Equal(Interpreter.toString("fss"), "1:1 Function 'f' is undefined");
        }


        [Fact]
        public void Test_ToCode() {
            Assert.Equal(Interpreter.toCode("sss"), new List<ExecutionStep>(){
                ExecutionStep.STEP_FORWARD,
                ExecutionStep.STEP_FORWARD,
                ExecutionStep.STEP_FORWARD,
            });
        }

        [Fact]
        public void Test_ToCode_Exception() {

            LanguageError ex = Assert.Throws<LanguageError>(() => Interpreter.toCode("f"));

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
                Interpreter.toString(code)
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
                Interpreter.toString(code)
            );

        }

        [Fact]
        public void EvalProgram_21() {
            var h = @"
f(A):A
f(ss)";

            Assert.Equal("ss",
            Interpreter.toString(h)
            );
        }

        
        [Fact]
        public void EvalProgram_22() {
            var h = @"
f(A, B):AB
f(ss, rr)";

            Assert.Equal("ssrr", Interpreter.toString(h));
        }


        [Fact]
        public void EvalProgram_3() {

            var h = @"
z:ll
f:rrz
fsss";
            
            Assert.Equal("rrllsss", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_4() {

            var h = @"
f(A):A
f(ss)rr";
            
            Assert.Equal("ssrr", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_5() {

            var h = @"
f(A,B):AB
f(ss,ll)rr";

            Assert.Equal("ssllrr", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_6() {

            var h = @"
            z:sss
            f(A,B):AzzzB
            f(ss,ll)rr";
            
            Assert.Equal("sssssssssssllrr", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_7() {
            var h = @"
z:sss
f(A):lA
f(z)rr";

            Assert.Equal("lsssrr", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_8() {
            var h = @"
z:sss
f(A):lA
f(ss)r";
            
            Assert.Equal("lssr", Interpreter.toString(h));
        }


        [Fact]
        public void EvalProgram_10() {
            var h = @"
f:f
f";
            
            Assert.Equal("", Interpreter.toString(h));
        }
        [Fact]
        public void EvalProgram_11() {
            var h = @"
f(A):f(A)s
f(0)";
            
            Assert.Equal("", Interpreter.toString(h));
        }

        [Fact]
        public void EvalProgram_12() {
            var h = @"
f(A):sf(A-1)
f(2)";
            
            Assert.Equal("ss", Interpreter.toString(h));
        }
        
        [Fact]
        public void EvalProgram_13() {
            var h = @"
f(A):f(A-1)s
f(2)";
            
            Assert.Equal("ss", Interpreter.toString(h));
        }
        
        [Fact]
        public void EvalProgram_14() {
            var h = @"
f(A):ss
f(2)";
            
            Assert.Equal("ss", Interpreter.toString(h));
        }
        
        [Fact]
        public void EvalProgram_15() {
            var h = @"

sss

";
            
            Assert.Equal("sss", Interpreter.toString(h));
        }
        
        [Fact]
        public void EvalProgram_16() {
            var h = @"
f:f";
            
            Assert.Equal("", Interpreter.toString(h));
        }

    }
}