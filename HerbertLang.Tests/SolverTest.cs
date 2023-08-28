using Xunit;


namespace HerbertLang.Tests;

public class SolverTest {

    [Fact]
    public void TestFindPlayer() {

        string[,] map = {
            {"#", "+", "#"},
            {"#", "_", "#"},
            {"#", "⇧", "#"},
        };

        var world = new World(map);
        Assert.Equal(0, world.player.direction);        
        Assert.Equal(1, world.player.position.x);        
        Assert.Equal(2, world.player.position.y);
    }

    [Fact]
    public void MoveForward() {
        string[,] map = {
            {"#", "+", "#"},
            {"#", "_", "#"},
            {"#", "⇧", "#"},
        };

        var world = new World(map);
        Assert.True(world.movePlayerForward());
        Assert.True(world.movePlayerForward());
        Assert.False(world.movePlayerForward());
    }


    [Fact]
    public void solve_1() {

        string[,] world = {
            {"#", "★", "#"},
            {"#", "_", "#"},
            {"#", "⇧", "#"},
        };

        var solution = Solver.Solve("ss", world);

        Assert.Equal(2, solution.steps.Length);

        Assert.Equal(Step.STEP_FORWARD, solution.steps[0]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[1]);
        
        Assert.True(solution.success);
    }

    [Fact]
    public void solve_2() {

        string[,] world = {
            {"#", "★", "#"},
            {"#", "_", "#"},
            {"#", "⇩", "#"},
        };

        var solution = Solver.Solve("rrss", world);

        Assert.Equal(4, solution.steps.Length);

        Assert.Equal(Step.TURN_RIGHT, solution.steps[0]);
        Assert.Equal(Step.TURN_RIGHT, solution.steps[1]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[2]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[3]);
        
        Assert.True(solution.success);
    }

    [Fact]
    public void solve_3() {

        string[,] world = {
            {"#", "#", "★"},
            {" ", " ", " "},
            {"⇧", "#", "#"},
        };

        var solution = Solver.Solve("srssls", world);

        Assert.Equal(6, solution.steps.Length);

        Assert.Equal(Step.STEP_FORWARD, solution.steps[0]);
        Assert.Equal(Step.TURN_RIGHT,   solution.steps[1]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[2]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[3]);
        Assert.Equal(Step.TURN_LEFT,    solution.steps[4]);
        Assert.Equal(Step.STEP_FORWARD, solution.steps[5]);
        
        Assert.True(solution.success);
    }

    [Fact]
    public void solveFail() {

        string[,] world = {
            {"#", "★", "#"},
            {"#", "_", "#"},
            {"#", "⇩", "#"},
        };

        var solution = Solver.Solve("ss", world);

        Assert.Equal(2, solution.steps.Length);

        Assert.Equal(Step.STEP_BAD, solution.steps[0]);
        Assert.Equal(Step.STEP_BAD, solution.steps[1]);
        
        Assert.False(solution.success);
    }
}
