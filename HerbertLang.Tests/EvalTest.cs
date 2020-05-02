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

            Assert.Equal(evalCode.steps.Count, 3);
            Assert.Equal((evalCode.steps[0] as Step).column, 1);
            Assert.Equal((evalCode.steps[1] as Step).column, 2);
            Assert.Equal((evalCode.steps[2] as Step).column, 3);

            Assert.Equal((evalCode.steps[0] as Step).line, 1);
            Assert.Equal((evalCode.steps[1] as Step).line, 1);
            Assert.Equal((evalCode.steps[2] as Step).line, 1);
        }

        [Fact]
        public void EvalFunctionDefinition() {
            var tokens = Lexer.tokenize("f:sr");
            var parser = new Parser(tokens);

            var code = parser.parseFunctionDefinition();
            var evalFunctionDefinition = code.eval() as FunctionDefinition;

            Assert.Equal(evalFunctionDefinition.name, "f");
            Assert.Equal(evalFunctionDefinition.column, 1);
            Assert.Equal(evalFunctionDefinition.line, 1);

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

            Assert.Equal(evalFunctionDefinition.name, "f");
            Assert.Equal(evalFunctionDefinition.column, 1);
            Assert.Equal(evalFunctionDefinition.line, 1);
            
            Assert.IsType<Step>(evalFunctionDefinition.next.node);
            Assert.IsType<Step>(evalFunctionDefinition.next.next.node);
            Assert.IsType<Variable>(evalFunctionDefinition.next.next.next.node);
            Assert.IsType<Variable>(evalFunctionDefinition.next.next.next.next.node);
        }

        [Fact]
        public void EvalProgram_1() {
            var tokens = Lexer.tokenize("rr \n lsr");
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;

            var code = program.eval() as Code;

            Assert.Equal(code.steps.Count, 5);
            Assert.Equal("rrlsr", program.compile());
        }

        [Fact]
        public void EvalProgram_2() {
            var tokens = Lexer.tokenize("f:rr\n fsss");
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;

            var code = program.eval() as Code;

            Assert.Equal(code.steps.Count, 5);
            Assert.Equal("rrsss", program.compile());
        }

        [Fact]
        public void EvalProgram_21() {
            var h = @"
f(A):A
f(ss)";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("ss", program.compile());
        }
        
        [Fact]
        public void EvalProgram_22() {
            var h = @"
f(A, B):AB
f(ss, rr)";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("ssrr", program.compile());
        }
        
        [Fact]
        public void EvalProgram_3() {

            var h = @"
            z:ll
            f:rrz
            fsss";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;

            var code = program.eval() as Code;

            Assert.Equal(code.steps.Count, 7);
            Assert.Equal("rrllsss", program.compile());
        }

        [Fact]
        public void EvalProgram_4() {

            var h = @"
            f(A):A
            f(ss)rr";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;

            var code = program.eval() as Code;

            Assert.Equal(4, code.steps.Count);
            Assert.Equal("ssrr", program.compile());
        }

        [Fact]
        public void EvalProgram_5() {

            var h = @"
            f(A,B):AB
            f(ss,ll)rr";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            var code = program.eval() as Code;

            Assert.Equal(6, code.steps.Count);
        }

        [Fact]
        public void EvalProgram_6() {

            var h = @"
            z:sss
            f(A,B):AzzzB
            f(ss,ll)rr";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("sssssssssssllrr", program.compile());
        }

        [Fact]
        public void EvalProgram_7() {
            var h = @"
z:sss
f(A):lA
f(z)rr";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("lsssrr", program.compile());
        }

        [Fact]
        public void EvalProgram_8() {
            var h = @"
z:sss
f(A):lA
f(ss)r";

            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("lssr", program.compile());
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

        [Fact]
        public void EvalProgram_10() {
            var h = @"
f:f
f";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            var output = program.compile();
            
            Assert.Equal(output, "");
        }
        [Fact]
        public void EvalProgram_11() {
            var h = @"
f(A):f(A)s
f(0)";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("", program.compile());
        }

        [Fact]
        public void EvalProgram_12() {
            var h = @"
f(A):sf(A-1)
f(2)";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("ss", program.compile());
        }
        
        [Fact]
        public void EvalProgram_13() {
            var h = @"
f(A):f(A-1)s
f(2)";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);

            var program = parser.parseProgram() as Program;
            Assert.Equal("ss", program.compile());
        }
        
        [Fact]
        public void EvalProgram_14() {
            var h = @"
f(A):ss
f(2)";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);
            var program = parser.parseProgram() as Program;
            Assert.Equal("ss", program.compile());
        }
        
        [Fact]
        public void EvalProgram_15() {
            var h = @"

sss

";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);
            var program = parser.parseProgram() as Program;
            Assert.Equal("sss", program.compile());
        }
        
        [Fact]
        public void EvalProgram_16() {
            var h = @"
f:f";
            var tokens = Lexer.tokenize(h);
            var parser = new Parser(tokens);
            var program = parser.parseProgram() as Program;
            Assert.Equal("", program.compile());
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
    }
}
