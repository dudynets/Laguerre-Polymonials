using Laguerre;

namespace LaguerrePolynomialsTests;

public class LaguerreSolverFixture : IDisposable
{
    public LaguerreSolver laguerreSolver;

    public LaguerreSolverFixture()
    {
        laguerreSolver = new LaguerreSolver(2, 4);
    }

    public void Dispose()
    {
        laguerreSolver = null;
    }
}

public class LaguerreSolverTests : IClassFixture<LaguerreSolverFixture>
{
    LaguerreSolver laguerreSolver;

    public LaguerreSolverTests(LaguerreSolverFixture fixture)
    {
        laguerreSolver = fixture.laguerreSolver;
    }

    [Fact]
    public void SolvePolynomial_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => laguerreSolver.SolvePolynomial(1, -1));
    }

    [Theory]
    [InlineData(1, 0, 0.7359)]
    [InlineData(1, 1, -2.2069)]
    [InlineData(1, 2, 0.7359)]
    [InlineData(1, 3, 1.717)]
    [InlineData(3, 0, 0.1)]
    public void SolvePolynomial_ShouldReturnCorrectValue(double t, int n, double expected)
    {
        double result = laguerreSolver.SolvePolynomial(t, n);
        Assert.Equal(expected, result, 3);
    }

    [Theory]
    [InlineData(-1, 1, 0.1)]
    [InlineData(1, -1, 0.1)]
    [InlineData(1, 1, -0.1)]
    public void TabulatePolynomial_ShouldThrowArgumentException(int n, double maxT, double tStep)
    {
        Assert.Throws<ArgumentException>(() => laguerreSolver.TabulatePolynomial(n, maxT, tStep));
    }

    [Theory]
    [InlineData(1, 0, 0.1, 0)]
    [InlineData(1, 2, 0.1, 20)]
    [InlineData(1, 3, 0.1, 30)]
    [InlineData(1, 4, 0.1, 40)]
    public void TabulatePolynomial_ShouldHaveCorrectLength(int n, double maxT, double tStep, int expected)
    {
        var result = laguerreSolver.TabulatePolynomial(n, maxT, tStep);
        Assert.Equal(expected, result.Count);
    }

    [Theory]
    [InlineData(-20, 1e-3, 100, 1000)]
    [InlineData(5, -1e-3, 100, 1000)]
    [InlineData(5, 1e-3, -100, 1000)]
    [InlineData(5, 1e-3, 100, -1000)]
    public void FindOptimalT_ShouldThrowArgumentException(int maxN, double epsilon, double maxT, int tPoints)
    {
        Assert.Throws<ArgumentException>(() => laguerreSolver.FindOptimalT(maxN, epsilon, maxT, tPoints));
    }

    [Theory]
    [InlineData(5, 1e-3, 100, 1000, 25.8258)]
    [InlineData(20, 1e-3, 100, 1000, 79.0789)]
    public void FindOptimalT_ShouldReturnCorrectValue(int maxN, double epsilon, double maxT, int tPoints, double expected)
    {
        double value = laguerreSolver.FindOptimalT(maxN, epsilon, maxT, tPoints).Key;
        Assert.Equal(expected, value, 3);
    }

    [Theory]
    [InlineData(1, 0.1, 1, 1000, 2)]
    [InlineData(10, 0.1, 1, 10000, 11)]
    [InlineData(100, 0.1, 1, 100000, 101)]
    public void FindOptimalT_TabulationShouldHaveCorrectLength(int maxN, double epsilon, double maxT, int tPoints, double expected)
    {
        Dictionary<int, double> tabulation = laguerreSolver.FindOptimalT(maxN, epsilon, maxT, tPoints).Value;
        Assert.Equal(expected, tabulation.Count);
    }

    [Theory]
    [InlineData(-5, 10)]
    [InlineData(5, -10)]
    public void SolveLaguerreTransform_ShouldThrowArgumentException(int maxN, int points)
    {
        Func<double, double> F = (double x) => Math.Pow(x, 2);
        Assert.Throws<ArgumentException>(() => laguerreSolver.SolveLaguerreTransform(F, maxN, points));
    }

    [Theory]
    [InlineData(5, 10, 0.403)]
    [InlineData(10, 10, 0.26)]
    [InlineData(15, 10, -0.0919)]
    public void SolveLaguerreTransform_ShouldReturnCorrectValueForBasicFunction(int maxN, int points, double expected)
    {
        Func<double, double> F = (double x) => Math.Pow(x, 2);

        double result = laguerreSolver.SolveLaguerreTransform(F, maxN, points);
        Assert.Equal(expected, result, 3);
    }

    [Theory]
    [InlineData(5, 10, 0, 1, 2.0619)]
    [InlineData(10, 10, 0, 1, 3.474)]
    [InlineData(15, 10, 0, 1, 4.8879)]
    public void SolveLaguerreTransform_ShouldReturnCorrectValueForNormalGaussianDistributionFunction(int maxN, int points, double gaussianMu, double gaussianLambda, double expected)
    {
        Func<double, double> NORMAL_GAUSSIAN_DISTRIBUTION = (double x) =>
            Math.Exp(
                -Math.Pow(x - gaussianMu, 2) /
                (2 * Math.Pow(gaussianLambda, 2))
            ) / (gaussianLambda * Math.Sqrt(2 * Math.PI));

        double result = laguerreSolver.SolveLaguerreTransform(NORMAL_GAUSSIAN_DISTRIBUTION, maxN, points);
        Assert.Equal(expected, result, 3);
    }

    [Theory]
    [InlineData(-5, 10)]
    [InlineData(5, -10)]
    public void TabulateLaguerreTransform_ShouldThrowArgumentException(int maxN, int points)
    {
        Func<double, double> F = (double x) => Math.Pow(x, 2);
        Assert.Throws<ArgumentException>(() => laguerreSolver.TabulateLaguerreTransform(F, maxN, points));
    }

    [Theory]
    [InlineData(5, 10, 5)]
    [InlineData(10, 10, 10)]
    [InlineData(15, 10, 15)]
    public void TabulateLaguerreTransform_ShouldHaveCorrectLength(int maxN, int points, int expected)
    {
        Func<double, double> F = (double x) => Math.Pow(x, 2);

        Dictionary<int, double> tabulation = laguerreSolver.TabulateLaguerreTransform(F, maxN, points);
        Assert.Equal(expected, tabulation.Count);
    }

    [Theory]
    [InlineData(1, 0.7369)]
    [InlineData(2, 5.846)]
    [InlineData(3, 9.957)]
    public void SolveInverseLaguerreTransform_ShouldReturnCorrectValueForBasicFunction(double t, double expected)
    {
        Func<double, double> F = (double x) => Math.Pow(x, 2);

        Dictionary<int, double> tabulation = laguerreSolver.TabulateLaguerreTransform(F, 5, 10);
        double[] h = tabulation.Values.ToArray();

        double result = laguerreSolver.SolveInverseLaguerreTransform(h, t);
        Assert.Equal(expected, result, 3);
    }
}
