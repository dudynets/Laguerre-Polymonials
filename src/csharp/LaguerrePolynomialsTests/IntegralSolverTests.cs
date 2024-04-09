using Laguerre;

namespace LaguerrePolynomialsTests;

public class IntegralSolverFixture : IDisposable
{
    public IntegralSolver integralSolver;

    public IntegralSolverFixture()
    {
        integralSolver = new IntegralSolver(x => Math.Pow(x, 2));
    }

    public void Dispose()
    {
        integralSolver = null;
    }
}

public class IntegralSolverTests : IClassFixture<IntegralSolverFixture>
{
    IntegralSolver integralSolver;

    public IntegralSolverTests(IntegralSolverFixture fixture)
    {
        integralSolver = fixture.integralSolver;
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    public void Solve_ShouldThrowArgumentExceptionIfAIsGreaterThanB(double a, double b)
    {
        Assert.Throws<ArgumentException>(() => integralSolver.Solve(a, b));
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(1, 1, -10)]
    public void Solve_ShouldThrowArgumentExceptionIfPointsIsInvalid(double a, double b, int points)
    {
        Assert.Throws<ArgumentException>(() => integralSolver.Solve(a, b, points));
    }

    [Theory]
    [InlineData(0, 1, 10000, 0.3333)]
    [InlineData(0, 2, 10000, 2.6665)]
    [InlineData(0, 3, 100000, 9)]
    public void Solve_ShouldReturnCorrectValue(double a, double b, int points, double expected)
    {
        double result = integralSolver.Solve(a, b, points);
        Assert.Equal(expected, result, 3);
    }
}
