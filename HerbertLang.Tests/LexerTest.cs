using System;
using Xunit;

using HerberLanguage;

namespace HerbertLang.Tests {
    public class TestsLexer {

        [Fact]
        public void TestEmpty() {
            var tokens = Lexer.tokenize("");
            Assert.Equal(tokens.Count, 0);
        }

        [Fact]
        public void TestSteps() {
            var tokens = Lexer.tokenize("srl");

            Assert.Equal(tokens.Count, 3);
            Assert.Equal(tokens[0].type, "STEP");
            Assert.Equal(tokens[1].type, "STEP");
            Assert.Equal(tokens[2].type, "STEP");
        }

        [Fact]
        public void TestProgram() {
            var tokens = Lexer.tokenize("f:sss\nfss");

            Assert.Equal(tokens.Count, 9);
            Assert.Equal(tokens[0].type, "FUNCTION");
            Assert.Equal(tokens[1].type, ":");
            Assert.Equal(tokens[2].type, "STEP");
            Assert.Equal(tokens[3].type, "STEP");
            Assert.Equal(tokens[4].type, "STEP");
            Assert.Equal(tokens[5].type, "NEW_LINE");
            Assert.Equal(tokens[6].type, "FUNCTION");
            Assert.Equal(tokens[7].type, "STEP");
            Assert.Equal(tokens[8].type, "STEP");
        }
    }
}
