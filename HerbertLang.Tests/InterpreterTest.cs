using Xunit;

using HerberLanguage;


namespace HerbertLang.Tests
{
    public class TestInterpter
    {

        [Fact]
        public void Test_SimpleSteps()
        {
            Assert.Equal(Interpreter.execute("sss"), "sss");            
        }


    }
}