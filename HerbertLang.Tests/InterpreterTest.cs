using Xunit;

using HerberLanguage;
using System.Collections.Generic;

namespace HerbertLang.Tests
{
    public class TestInterpter
    {

        [Fact]
        public void Test_ToString()
        {
            Assert.Equal(Interpreter.toString("sss"), "sss");
        }

        [Fact]
        public void Test_ToString_Error()
        {
            Assert.Equal(Interpreter.toString("fss"), "1:1 Function 'f' is undefined");
        }


        [Fact]
        public void Test_ToCode()
        {
            Assert.Equal(Interpreter.toCode("sss"), new List<ExecutionStep>(){
                ExecutionStep.STEP_FORWARD,
                ExecutionStep.STEP_FORWARD,
                ExecutionStep.STEP_FORWARD,
            });
        }

        [Fact]
        public void Test_ToCode_Exception()
        {

            LanguageError ex = Assert.Throws<LanguageError>(() => Interpreter.toCode("f"));

            Assert.Equal(1, ex.line);
            Assert.Equal(1, ex.column);
        }

    }
}