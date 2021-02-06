using System;
using Xunit;

using HerberLang;

namespace HerbertLang.Tests {
    public class TestsLexer {

        [Fact]
        public void TestEmpty() {
            var tokens = Lexer.tokenize("");
            Assert.Empty(tokens);
        }

        [Fact]
        public void TestSteps() {
            var tokens = Lexer.tokenize("srl");

            Assert.Equal(3, tokens.Count);

            Assert.Equal("STEP", tokens[0].type);
            Assert.Equal("STEP", tokens[1].type);
            Assert.Equal("STEP", tokens[2].type);
        }

        [Fact]
        public void TestProgram() {
            var tokens = Lexer.tokenize("f:sss\nfss");

            Assert.Equal(9, tokens.Count);
            Assert.Equal("FUNCTION", tokens[0].type);
            Assert.Equal(":", tokens[1].type);
            Assert.Equal("STEP", tokens[2].type);
            Assert.Equal("STEP", tokens[3].type);
            Assert.Equal("STEP", tokens[4].type);
            Assert.Equal("NEW_LINE", tokens[5].type);
            Assert.Equal("FUNCTION", tokens[6].type); ;
            Assert.Equal("STEP", tokens[7].type);
            Assert.Equal("STEP", tokens[8].type);
        }
    }
}
