using System;
using Xunit;

using HerberLang;
using System.Collections.Generic;


namespace HerbertLang.Tests {

    // public class NextTest {

    //     [Fact]
    //     public void NextSumTest() {
    //         var next = new Next("+", new Number(6), new Next(new Number(6)));
    //         var number = next.eval() as Number;
    //         Assert.Equal(number.value, 12);
    //     }

    //     [Fact]
    //     public void NextNumberTest() {
    //         var next = new Next(new Number(6));
    //         var number = next.eval() as Number;
    //         Assert.Equal(number.value, 6);
    //     }

    //     [Fact]
    //     public void NextStepTest() {
    //         var next = new Next(new Step("s"));
    //         var code = next.eval() as Code;

    //         Assert.Equal(1, code.steps.Count);
    //     }

    //     [Fact]
    //     public void NextStepStepTest() {
    //         var next = new Next(new Step("s"), new Next(new Step("r")));
    //         var code = next.eval() as Code;

    //         Assert.Equal(2, code.steps.Count);
    //         Assert.Equal("s", (code.steps[0] as Step).step);
    //         Assert.Equal("r", (code.steps[1] as Step).step);
    //     }

    //     [Fact]
    //     public void NextStepVariableTest() {
    //         var varibles = new Dictionary<string, INode>() {
    //             {"A", new Code(new List<INode>(){new Step("r")})}
    //         };
    //         var stackFrame = new StackFrame(varibles);
    //         var context = new Context(stackFrame);

    //         var next = new Next(new Step("s"), new Next(new Variable("A")));
    //         var code = next.eval(context) as Code;

    //         Assert.Equal(2, code.steps.Count);
    //         Assert.Equal("s", (code.steps[0] as Step).step);
    //         Assert.Equal("r", (code.steps[1] as Step).step);
    //     }

    //     [Fact]
    //     public void NextStepVariableErrorTest() {
    //         var varibles = new Dictionary<string, INode>() {
    //             {"A", new Number(6)}
    //         };
    //         var stackFrame = new StackFrame(varibles);
    //         var context = new Context(stackFrame);

    //         var next = new Next(new Step("s"), new Next(new Variable("A")));

    //         LanguageError ex = Assert.Throws<LanguageError>(
    //             () => next.eval(context)
    //         );
    //     }

    // }

    // public class VariableTest {

    //     [Fact]
    //     public void TestNumericVariable() {

    //         var varibles = new Dictionary<string, INode>() {
    //             {"A", new Number(6)}
    //         };

    //         var stackFrame = new StackFrame(varibles);
    //         var context = new Context(stackFrame);
    //         var variable = new Variable("A");

    //         var number = variable.eval(context) as Number;

    //         Assert.Equal(number.value, 6);
    //     }

    //     [Fact]
    //     public void TestNumberVariableUndefined() {
    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);
    //         var variable = new Variable("A");

    //         try {
    //             variable.eval(context);
    //             Assert.True(false);
    //         } catch (LanguageError e) {
    //             Assert.Contains("undefined", e.Message);
    //         }
    //     }

    //     [Fact]
    //     public void TestCodeVariable() {

    //         var steps = new List<INode>() {
    //             new Step("s"),
    //             new Step("s")
    //         };
    //         var varibles = new Dictionary<string, INode>() {
    //             {"A", new Code(steps)}
    //         };

    //         var stackFrame = new StackFrame(varibles);
    //         var context = new Context(stackFrame);
    //         var variable = new Variable("A");

    //         var code = variable.eval(context) as Code;
    //         Assert.Equal(code.steps.Count, 2);
    //     }

    //     [Fact]
    //     public void TestCodeVariableUndefined() {
    //         var stackFrame = new StackFrame();

    //         var context = new Context(stackFrame);
    //         var variable = new Variable("A");

    //         try {
    //             variable.eval(context);
    //             Assert.True(false);
    //         } catch (LanguageError e) {
    //             Assert.Contains("undefined", e.Message);
    //         }
    //     }
    // }

    // public class FunctionCallTest {

    //     [Fact]
    //     public void CallSimple() {
    //         var code = new Code(new List<INode>() {
    //             new Step("s"),
    //             new Step("s")
    //         });

    //         var parameters = new List<INode>();
    //         var functionDefinition = new FunctionDefinition("f", parameters, code);

    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);

    //         context.definitions.Add("f", functionDefinition);

    //         var functionCall = new FunctionCall("f");
    //         var evalCode = functionCall.eval(context) as Code;

    //         Assert.Equal(evalCode.steps.Count, 2);
    //     }

    //     [Fact]
    //     public void CallArguments_1() {
    //         var code = new Code(new List<INode>() {new Step("s"), new Step("s")});
    //         var parameters = new List<INode>();
    //         var functionDefinition = new FunctionDefinition("f", parameters, code);

    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);

    //         context.definitions.Add("f", functionDefinition);

    //         var functionCall = new FunctionCall("f");
    //         var evalCode = functionCall.eval(context) as Code;

    //         Assert.Equal(evalCode.steps.Count, 2);
    //     }

    //     [Fact]
    //     public void CallArguments_2() {
    //         var definitionCode = new Code(new List<INode>() {
    //             new Step("s"), new Step("s"), new Variable("A")
    //         });

    //         var definitionParameters = new List<INode>(){
    //             new FunctionParameter("A")
    //         };
    //         var definition = new FunctionDefinition("f", definitionParameters, definitionCode);

    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);

    //         context.definitions.Add("f", definition);

    //         var callArguments = new List<INode>(){
    //             new Code(new List<INode>() {
    //                 new Step("s"), new Step("s")
    //             })
    //         };

    //         var call = new FunctionCall("f", callArguments);
    //         var evalCode = call.eval(context) as Code;

    //         Assert.Equal(evalCode.steps.Count, 4);
    //     }

    //     [Fact]
    //     public void CallArgumentsErrorMissing() {
    //         var definitionCode = new Code(new List<INode>() {
    //             new Step("s"), new Step("s"), new Variable("A")
    //         });

    //         var definitionParameters = new List<INode>(){
    //             new FunctionParameter("A")
    //         };

    //         var definition = new FunctionDefinition("f", definitionParameters, definitionCode);

    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);

    //         context.definitions.Add("f", definition);

    //         var callArguments = new List<INode>();

    //         var call = new FunctionCall("f", callArguments);

    //         try {
    //             call.eval(context);
    //             Assert.True(false);
    //         } catch (LanguageError e) {
    //             Assert.Contains("missing argument", e.Message);
    //         }
    //     }

    //     [Fact]
    //     public void CallArgumentsErrorTooMany() {
    //         var definitionCode = new Code(new List<INode>() {
    //             new Step("s"), new Step("s"), new Variable("A")
    //         });

    //         var definitionParameters = new List<INode>(){
    //             new FunctionParameter("A"),
    //         };

    //         var definition = new FunctionDefinition("f", definitionParameters, definitionCode);

    //         var stackFrame = new StackFrame();
    //         var context = new Context(stackFrame);

    //         context.definitions.Add("f", definition);

    //         var callArguments = new List<INode>(){
    //             new Code(new List<INode>() {new Step("s"), new Step("s")}),
    //             new Code(new List<INode>() {new Step("s"), new Step("s")})
    //         };

    //         var call = new FunctionCall("f", callArguments);

    //         try {
    //             call.eval(context);
    //             Assert.True(false);
    //         } catch (LanguageError e) {
    //             Assert.Contains("too many arguments", e.Message);
    //         }
    //     }
    // }
}