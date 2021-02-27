using System;
using System.Collections.Generic;
using System.Text;

namespace HerberLang {

    public static class Tile {
        public const string PLAYER_UP = "0";
        public const string PLAYER_RIGHT = "1";
        public const string PLAYER_DOWN = "2";
        public const string PLAYER_LEFT = "3";
        public const string OBSTACLE = "x";
        public const string SPACE = "_";
        public const string ITEM = "*";
    }

    public static class Step {
        public const char TURN_RIGHT = 'r';
        public const char TURN_LEFT = 'l';
        public const char STEP_FORWARD = 's';
        public const char STEP_BAD = 'x';
    }

    public class Solution {
        public bool success = false;
        public LanguageError error;

        public string steps;
        public string code;

        public Solution(bool success, string steps, string code) {
            this.success = success;
            this.steps = steps;
            this.code = code;
        }

        public Solution(LanguageError error) {
            this.error = error;
        }

    }

    public class Solver {

        public static Solution Solve(string codeInput, string[,] world) {

            int direction = 0;
            int[] position = new int[2];

            for (int i = 0; i <= world.GetUpperBound(0); i++) {
                for (int j = 0; j <= world.GetUpperBound(1); j++) {
                    if (world[i, j] == Tile.PLAYER_UP ||
                        world[i, j] == Tile.PLAYER_RIGHT ||
                        world[i, j] == Tile.PLAYER_DOWN ||
                        world[i, j] == Tile.PLAYER_LEFT) {
                        direction = int.Parse(world[i, j]);
                        position[0] = i;
                        position[1] = j;
                    }
                }
            }

            string interpreterSteps;

            try {
                interpreterSteps = Interpreter.evalToCode(codeInput);
            } catch (LanguageError error) {
                return new Solution(error);
            }

            var solutionSteps = new StringBuilder();

            foreach (var step in interpreterSteps) {

                if (step == Step.TURN_RIGHT) {
                    direction = (direction + 1) % 4;
                    solutionSteps.Append(Step.TURN_RIGHT);
                } else
                if (step == Step.TURN_LEFT) {
                    direction = (direction - 1) % 4;
                    if (direction < 0) {
                        direction += 4;
                    }
                    solutionSteps.Append(Step.TURN_LEFT);
                } else
                if (step == Step.STEP_FORWARD) {
                    int[] nextPosition = new int[] { position[0], position[1] };

                    if (direction == 0) {
                        nextPosition[0]--;
                    } else
                    if (direction == 1) {
                        nextPosition[1]++;
                    } else
                    if (direction == 2) {
                        nextPosition[0]++;
                    } else
                    if (direction == 3) {
                        nextPosition[1]--;
                    }

                    if (world[nextPosition[0], nextPosition[1]] == Tile.OBSTACLE) {
                        solutionSteps.Append(Step.STEP_BAD);
                    } else
                    if (nextPosition[0] > world.GetUpperBound(0) || nextPosition[0] < 0) {
                        solutionSteps.Append(Step.STEP_BAD);
                    } else
                    if (nextPosition[1] > world.GetUpperBound(1) || nextPosition[1] < 0) {
                        solutionSteps.Append(Step.STEP_BAD);
                    } else {
                        solutionSteps.Append(Step.STEP_FORWARD);

                        position[0] = nextPosition[0];
                        position[1] = nextPosition[1];

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

            return new Solution(
                success,
                solutionSteps.ToString(),
                codeInput
            );

        }
    }
}