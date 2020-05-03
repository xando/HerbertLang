using System;
using Xunit;

using HerberLanguage;
using System.Collections.Generic;


namespace HerbertLang.Tests {
    public class EvalTest {

        [Fact]
        public void EvalCode() {
            var tokens = Lexer.tokenize("sss");
            var parser = new Parser(tokens);

            var code = parser.parseNext();
            var evalCode = code.eval() as Code;

            Assert.Equal(3, evalCode.steps.Count);
            Assert.Equal(1, (evalCode.steps[0] as Step).column);
            Assert.Equal(2, (evalCode.steps[1] as Step).column);
            Assert.Equal(3, (evalCode.steps[2] as Step).column);

            Assert.Equal(1, (evalCode.steps[0] as Step).line);
            Assert.Equal(1, (evalCode.steps[1] as Step).line);
            Assert.Equal(1, (evalCode.steps[2] as Step).line);
        }

        [Fact]
        public void EvalFunctionDefinition() {
            var tokens = Lexer.tokenize("f:sr");
            var parser = new Parser(tokens);

            var code = parser.parseFunctionDefinition();
            var evalFunctionDefinition = code.eval() as FunctionDefinition;

            Assert.Equal("f", evalFunctionDefinition.name);
            Assert.Equal(1, evalFunctionDefinition.column);
            Assert.Equal(1, evalFunctionDefinition.line);

            Assert.NotNull(evalFunctionDefinition.next);
            Assert.NotNull(evalFunctionDefinition.next.next);
            Assert.Null(evalFunctionDefinition.next.next.next);

            Assert.Equal("s", (evalFunctionDefinition.next.node as Step).step);
            Assert.Equal("r", (evalFunctionDefinition.next.next.node as Step).step);
        }

        [Fact]
        public void EvalFunctionDefinitionArguments() {
            var tokens = Lexer.tokenize("f(A,B):ssAA");
            var parser = new Parser(tokens);

            var code = parser.parseFunctionDefinition();
            var evalFunctionDefinition = code.eval() as FunctionDefinition;

            Assert.Equal("f", evalFunctionDefinition.name);
            Assert.Equal(1, evalFunctionDefinition.column);
            Assert.Equal(1, evalFunctionDefinition.line);

            Assert.IsType<Step>(evalFunctionDefinition.next.node);
            Assert.IsType<Step>(evalFunctionDefinition.next.next.node);
            Assert.IsType<Variable>(evalFunctionDefinition.next.next.next.node);
            Assert.IsType<Variable>(evalFunctionDefinition.next.next.next.next.node);
        }

        [Fact]
        public void EvalProgram_17() {
            var h = "1+";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            LanguageError ex = Assert.Throws<LanguageError>(
                () => parser.parseNext());
            
            Assert.Equal(1, ex.line);
            Assert.Equal(2, ex.column);
        }

        [Fact]
        public void EvalProgram_9() {
            var h = @"
z:r
z:l";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            LanguageError ex = Assert.Throws<LanguageError>(() => parser.parseProgram());

            Assert.Equal(3, ex.line);
            Assert.Equal(1, ex.column);
        }


    }
}
