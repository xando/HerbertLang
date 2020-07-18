using Xunit;

using HerberLang;


namespace HerbertLang.Tests {

    public class SolverTest {
        // It says hhere PLAYER_BOTTOM but ...
        // we using array from the top to bottom

        [Fact]
        public void solve_1() {

            Tile[,] world = {
                {Tile.OBSTACLE, Tile.ITEM,       Tile.OBSTACLE},
                {Tile.OBSTACLE, Tile.SPACE,      Tile.OBSTACLE},
                {Tile.OBSTACLE, Tile.PLAYER_BOTTOM, Tile.OBSTACLE}
            };

            var execution = new Execution("sss");

            var solution = execution.solve(world);

            Assert.Equal(3, solution.steps.Count);

            Assert.Equal(ExecutionStep.STEP_FORWARD, solution.steps[0]);
            Assert.Equal(ExecutionStep.STEP_FORWARD, solution.steps[1]);
            Assert.Equal(ExecutionStep.STEP_BAD, solution.steps[2]);
            Assert.True(solution.success);
        }

        [Fact]
        public void solve_2() {
            Tile[,] world = {
                {Tile.OBSTACLE, Tile.ITEM,       Tile.OBSTACLE},
                {Tile.OBSTACLE, Tile.SPACE,      Tile.OBSTACLE},
                {Tile.OBSTACLE, Tile.PLAYER_BOTTOM, Tile.OBSTACLE}
            };

            var execution = new Execution("s");

            var solution = execution.solve(world);

            Assert.Single(solution.steps);

            Assert.Equal(ExecutionStep.STEP_FORWARD, solution.steps[0]);
            Assert.False(solution.success);
        }

    }


}