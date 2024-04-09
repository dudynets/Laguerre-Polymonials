namespace Laguerre
{
    public class IntegralSolver
    {
        public Func<double, double> f { get; set; }

        public IntegralSolver(Func<double, double> _f)
        {
            f = _f;
        }

        public double Solve(double a, double b, int points = 10000)
        {
            if (a > b)
                throw new ArgumentException("Value \"a\" must be less than \"b\"");

            if (points <= 0)
                throw new ArgumentException("Value \"points\" must be positive");

            double[] x = new double[points];
            double step = (b - a) / points;
            double s = 0;

            for (int i = 0; i < points; i++)
            {
                x[i] = a + i * step;
                s += f(x[i]);
            }

            double result = s * Math.Abs(b - a) / points;
            return result;
        }
    }
}
