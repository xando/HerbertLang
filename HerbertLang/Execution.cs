using System;
using System.Collections.Generic;


namespace HerberLang {

    public enum Tile {
        PLAYER_TOP = 0,
        PLAYER_RIGHT = 1,
        PLAYER_BOTTOM = 2,
        PLAYER_LEFT = 3,
        SPACE = 4,
        OBSTACLE = 5,
        ITEM = 9,
    }

    public enum ExecutionStep {
        TURN_RIGHT,
        TURN_LEFT,
        STEP_FORWARD,
        STEP_BAD,
    };

    public class Solution {
        public bool success = false;
        public LanguageError error;

        public List<ExecutionStep> steps;
        public string code;

        public Solution(bool success, List<ExecutionStep> steps, string code) {
            this.success = success;
            this.steps = steps;
            this.code = code;
        }

        public Solution(LanguageError error) {
            this.error = error;
        }

    }

    public class Execution {

        string codeInput;

        public Execution(string codeInput) {
            this.codeInput = codeInput;
        }

        public Solution solve(Tile[,] world) {

            int direction = 0;
            int[] position = new int[2];

            for (int i = 0; i <= world.GetUpperBound(0); i++) {
                for (int j = 0; j <= world.GetUpperBound(1); j++) {
                    if (world[i, j] == Tile.PLAYER_TOP ||
                        world[i, j] == Tile.PLAYER_RIGHT ||
                        world[i, j] == Tile.PLAYER_BOTTOM ||
                        world[i, j] == Tile.PLAYER_LEFT) {
                        direction = (int)world[i, j];
                        position[0] = i;
                        position[1] = j;
                    }
                }
            }

            List<ExecutionStep> steps;
            try {
                steps = Interpreter.evalToCode(codeInput);
            } catch (LanguageError error) {
                return new Solution(error);
            }

            List<ExecutionStep> solutionSteps = new List<ExecutionStep>();

            foreach (var step in steps) {
                if (step == ExecutionStep.TURN_RIGHT) {
                    direction = (direction + 1) % 4;
                    solutionSteps.Add(ExecutionStep.TURN_RIGHT);
                } else
                if (step == ExecutionStep.TURN_LEFT) {
                    direction = (direction - 1) % 4;
                    if (direction < 0) {
                        direction += 4;
                    }
                    solutionSteps.Add(ExecutionStep.TURN_LEFT);
                } else
                if (step == ExecutionStep.STEP_FORWARD) {
                    int[] nextPosition = new int[2];

                    position.CopyTo(nextPosition, 0);


                    if (direction == 0) {
                        nextPosition[0]++;
                    } else
                    if (direction == 1) {
                        nextPosition[1]--;
                    } else
                    if (direction == 2) {
                        nextPosition[0]--;
                    } else
                    if (direction == 3) {
                        nextPosition[1]++;
                    }

                    if (nextPosition[0] > world.GetUpperBound(0) ||
                        nextPosition[0] < 0) {
                        solutionSteps.Add(ExecutionStep.STEP_BAD);
                    } else
                    if (nextPosition[1] > world.GetUpperBound(1) ||
                        nextPosition[1] < 0) {
                        solutionSteps.Add(ExecutionStep.STEP_BAD);
                    } else {
                        solutionSteps.Add(ExecutionStep.STEP_FORWARD);
                        nextPosition.CopyTo(position, 0);

                        if (world[position[0], position[1]] == Tile.ITEM) {
                            world[position[0], position[1]] = Tile.SPACE;
                        }

                    }
                }
            }

            bool success = true;
            for (int i = 0; i <= world.GetUpperBound(0); i++) {
                for (int j = 0; j <= world.GetUpperBound(1); j++) {
                    if (world[i, j] == Tile.ITEM) {
                        success = false;
                        break;
                    }
                }
            }

            return new Solution(success, solutionSteps, this.codeInput);
        }
    }
}