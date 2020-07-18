using System;
using System.Diagnostics;
using HerberLang;


public class Herbert
{
    static string[] args;
    static int total = 0;

    static int passed = 0;

    static void check(string name, string program, ExecutionStep[] expected) {
        if (args.Length > 0 && args[0] != name) {
            return;
        }

        var output = Interpreter.evalToCode(program).ToArray();

        int biggerSize;
        if (expected.Length > output.Length) {
            biggerSize = expected.Length;
        } else {
            biggerSize = output.Length;
        }


        bool success = true;
        if (output.Length != expected.Length) {
            success = false;
        }
        if (success) {
            for (int i = 0; i < expected.Length; i++) {
                if (output[i] != expected[i]) {
                     success = false;
                     break;
                }
            }
        }

        var expectedAnnotated = new Tuple<bool, ExecutionStep>[expected.Length];
        var outputAnnotated = new Tuple<bool, ExecutionStep>[biggerSize];

        for (int i = 0; i < expected.Length; i++) {
            expectedAnnotated[i] = Tuple.Create(
                i < output.Length && output[i] == expected[i],
                expected[i]
            );
        }

        for (int i = 0; i < output.Length; i++) {
            outputAnnotated[i] = Tuple.Create(
                i < expected.Length && output[i] == expected[i],
                output[i]
            );
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\n### {0}: ", name);

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("PASSED");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("FAILED");
        }
        Console.Write("\n");
        Console.ResetColor();

        Console.WriteLine(program);

        if (!success) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Format("{0,-18}{1}", "Output", "Expected"));
            Console.ResetColor();

            for (int i = 0; i < biggerSize; i++) {
                if (i < output.Length) {
                    if (i >= expected.Length || output[i] != expected[i]) {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(String.Format("{0, -18}", output[i]));
                    Console.ResetColor();
                }
                if (i < expected.Length) {
                    if (i >= output.Length || output[i] != expected[i]) {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(expected[i]);
                    Console.ResetColor();
                }
                Console.WriteLine("");
            }
        }

        total++;
        if (success)
        {
            passed++;
        }

    }

    public static void solve(string name, Tile[,] world, string code) {
        if (args.Length > 0 && args[0] != name) {
            return;
        }

        var execution = new Execution(code);
        var solution = execution.solve(world);

        if (!solution.success) {

            for (int i = 0; i <= world.GetUpperBound(0); i++) {
                for (int j = 0; j <= world.GetUpperBound(1); j++) {

                    if (world[i, j] == Tile.PLAYER_TOP ||
                        world[i, j] == Tile.PLAYER_RIGHT ||
                        world[i, j] == Tile.PLAYER_BOTTOM ||
                        world[i, j] == Tile.PLAYER_LEFT) {
                        Console.Write(String.Format("{0} ", (int)world[i, j]));
                    } else
                    if (world[i, j] == Tile.ITEM) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(String.Format("{0} ", (char)world[i, j]));
                        Console.ResetColor();
                    } else {
                        Console.Write(String.Format("{0} ", (char)world[i, j]));
                    }
                }
                Console.WriteLine("");
            }
        }

        total++;
        if (solution.success)
        {
            passed++;
        }
    }

    static public void Main(string[] args)
    {
        Herbert.args = args;

        solve("solve no items",
            new Tile[,] {
                {Tile.SPACE, Tile.SPACE, Tile.SPACE},
                {Tile.SPACE, Tile.PLAYER_TOP, Tile.SPACE},
                {Tile.SPACE, Tile.SPACE, Tile.SPACE},
            },
            "s"
        );

        solve("solve step",
            new Tile[,] {
                {Tile.SPACE, Tile.SPACE, Tile.SPACE},
                {Tile.SPACE, Tile.PLAYER_TOP, Tile.SPACE},
                {Tile.SPACE, Tile.ITEM, Tile.SPACE},
            },
            "s"
        );

        solve("solve step turn",
            new Tile[,] {
                {Tile.SPACE, Tile.SPACE, Tile.SPACE},
                {Tile.SPACE, Tile.PLAYER_TOP, Tile.SPACE},
                {Tile.ITEM, Tile.SPACE, Tile.SPACE},
            },
            "srs"
        );

        solve("solve step turn",
            new Tile[,] {
                {Tile.ITEM, Tile.SPACE, Tile.ITEM},
                {Tile.SPACE, Tile.PLAYER_TOP, Tile.SPACE},
                {Tile.ITEM, Tile.SPACE, Tile.ITEM},
            },
            "srsrssrssrss"
        );

        check("simple 1", "s", new ExecutionStep[] { ExecutionStep.STEP_FORWARD });

        check("simple 2", "srl", new ExecutionStep[] {
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.TURN_LEFT
        });

        check("multiline 1", "srl\nsss", new ExecutionStep[] {
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.TURN_LEFT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD
        });

        check("multiline 2", "\nsss\nsss\n\n", new ExecutionStep[] {
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD
        });

        check("function 1", "f:s", new ExecutionStep[] {});
        check("function 2", "f:s\nf", new ExecutionStep[] {
            ExecutionStep.STEP_FORWARD });
        check("function 3", "f:s\nf", new ExecutionStep[] {
            ExecutionStep.STEP_FORWARD });
        check("function 4", "f:s\nff", new ExecutionStep[]{
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD
        });

        check("function arguments", "f(A):AsA\nf(r)", new ExecutionStep[]{
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT
        });

        check("function arguments", "f(A, B):ABsBA\nf(r,l)", new ExecutionStep[]{
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.TURN_LEFT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_LEFT,
            ExecutionStep.TURN_RIGHT
        });

        check("recursion load = 0", "f:sf\nf=0", new ExecutionStep[]{});
        check("recursion load - 1", "f:sf\nf-1", new ExecutionStep[]{});
        check("recursion load =- 1", "f:sf\nf=-1", new ExecutionStep[]{});
        check("recursion load = 1", "f:sf-1\nf=1", new ExecutionStep[]{
            ExecutionStep.STEP_FORWARD
        });

        check("recursion load = 2", "f:sf-1\nf=2", new ExecutionStep[]{
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD
        });

        check("multi recursion load = 1", "z:srz-1\nf:sz-1f-1\nf=4", new ExecutionStep[]{
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.STEP_FORWARD,
            ExecutionStep.TURN_RIGHT,
            ExecutionStep.STEP_FORWARD
        });

        Console.WriteLine("");
        Console.WriteLine("Total: {0}, Passed: {1}", total, passed);
    }
}