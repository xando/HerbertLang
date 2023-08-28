using System.Text;

namespace HerbertLang;


public static class Step {
    public const char TURN_RIGHT = 'r';
    public const char TURN_LEFT = 'l';
    public const char STEP_FORWARD = 's';
    public const char STEP_BAD = 'x';
}


public struct Solution {

    public bool success;
    public string steps;
    public string code;

}

public class Solver {

    public static Solution Solve(string code, string[,] map) {

        var world = new World(map);

        var solutionSteps = new StringBuilder();

        foreach (var step in code) {

            if (step == Step.TURN_RIGHT) {
                world.turnPlayerRight();
                solutionSteps.Append(Step.TURN_RIGHT);
            } else
            if (step == Step.TURN_LEFT) {
                world.turnPlayerLeft();
                solutionSteps.Append(Step.TURN_LEFT);
            } else
            if (step == Step.STEP_FORWARD) {
                if (world.movePlayerForward()) {
                    solutionSteps.Append(Step.STEP_FORWARD);
                } else {
                    solutionSteps.Append(Step.STEP_BAD);
                }
            }
        }

        bool success = world.isSolved();

        return new Solution { 
            success = success,
            steps = solutionSteps.ToString(),
            code = code
        };
    }
}
