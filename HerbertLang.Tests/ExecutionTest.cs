using Xunit;

using HerberLanguage;


namespace HerbertLang.Tests {
    public class ExecutionTest {

        [Fact]
        public void StepTest_1() {
            var code = "sss";
            var execution = new Execution(code);
            var steps = execution.getSteps();

            Assert.Equal(3, steps.Count);

            Assert.Equal(ExecutionStep.STEP_FORWARD, steps[0]);
            Assert.Equal(ExecutionStep.STEP_FORWARD, steps[1]);
            Assert.Equal(ExecutionStep.STEP_FORWARD, steps[2]);
        }

        [Fact]
        public void StepTest_2() {
            var code = "srl";
            var execution = new Execution(code);
            var steps = execution.getSteps();

            Assert.Equal(3, steps.Count);

            Assert.Equal(ExecutionStep.STEP_FORWARD, steps[0]);
            Assert.Equal(ExecutionStep.TURN_RIGHT, steps[1]);
            Assert.Equal(ExecutionStep.TURN_LEFT, steps[2]);
        }
    }

    public class SolveTest {
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
            
            Assert.Equal(solution.steps.Count, 3);
            
            Assert.Equal(solution.steps[0], ExecutionStep.STEP_FORWARD);
            Assert.Equal(solution.steps[1], ExecutionStep.STEP_FORWARD);
            Assert.Equal(solution.steps[2], ExecutionStep.STEP_BAD);
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
            
            Assert.Equal(solution.steps.Count, 1);
            
            Assert.Equal(solution.steps[0], ExecutionStep.STEP_FORWARD);
            Assert.False(solution.success);
        }

    }


}