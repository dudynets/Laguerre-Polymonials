namespace Laguerre
{
    public class LaguerreSolver
    {
        public double beta { get; set; }
        public double sigma { get; set; }

        public LaguerreSolver(double _beta = 2.0, double _sigma = 4.0)
        {
            if (_beta < 0)
                throw new ArgumentException("Value \"beta\" must be positive");

            if (_sigma < _beta)
                throw new ArgumentException("Value \"sigma\" must be greater than beta");

            beta = _beta;
            sigma = _sigma;
        }

        public double SolvePolynomial(double t, int n)
        {
            // Validation of input data
            if (n < 0)
                throw new ArgumentException("Value \"n\" must be positive");

            // Best cases
            double lPrevPrev = Math.Sqrt(sigma) * Math.Exp(-beta * t / 2);
            double lPrev = Math.Sqrt(sigma) * (1 - sigma * t) * Math.Exp(-beta * t / 2);
            if (n == 0)
                return lPrevPrev;
            if (n == 1)
                return lPrev;

            // Computation
            for (int i = 2; i <= n; i++)
            {
                double temp = lPrev;
                lPrev = (2 * i - 1 - sigma * t) * lPrev / i - (i - 1) * lPrevPrev / i;
                lPrevPrev = temp;
            }

            return lPrev;
        }

        public Dictionary<double, double> TabulatePolynomial(int n, double maxT, double tStep = 0.1)
        {
            if (n < 0)
                throw new ArgumentException("Value \"n\" must be positive");

            if (maxT < 0)
                throw new ArgumentException("Value \"maxT\" must be positive");

            if (tStep < 0)
                throw new ArgumentException("Value \"tStep\" must be positive");

            var result = new Dictionary<double, double>();
            for (double t = 0; t < maxT; t += tStep)
            {
                result.Add(t, SolvePolynomial(t, n));
            }

            return result;
        }

        public KeyValuePair<double, Dictionary<int, double>> FindOptimalT(int maxN = 20, double epsilon = 1e-3, double maxT = 100, int tPoints = 1000)
        {
            Console.WriteLine($"maxN: {maxN}, epsilon: {epsilon}, maxT: {maxT}, tPoints: {tPoints}");
            if (maxN < 0)
                throw new ArgumentException("Value \"N\" must be positive");

            if (epsilon < 0)
                throw new ArgumentException("Value \"epsilon\" must be positive");

            if (maxT < 0)
                throw new ArgumentException("Value \"maxT\" must be positive");

            if (tPoints < 0)
                throw new ArgumentException("Value \"tPoints\" must be positive");

            var result = new Dictionary<int, double>();
            double suitableT = 0;

            double[] T = new double[tPoints];
            for (int i = 0; i < tPoints; i++)
            {
                T[i] = i * (maxT / (tPoints - 1));
            }

            for (int tIndex = 0; tIndex < tPoints; tIndex++)
            {
                bool isTSuitable = true;
                for (int n = 0; n <= maxN; n++)
                {
                    if (Math.Abs(SolvePolynomial(T[tIndex], n)) > epsilon)
                    {
                        isTSuitable = false;
                        break;
                    }
                }

                if (isTSuitable)
                {
                    suitableT = T[tIndex];
                    break;
                }
            }

            for (int n = 0; n <= maxN; n++)
            {
                result.Add(n, SolvePolynomial(suitableT, n));
            }

            return new KeyValuePair<double, Dictionary<int, double>>(suitableT, result);
        }

        public double SolveLaguerreTransform(Func<double, double> f, int maxN, int points = 10000)
        {
            if (maxN < 0)
                throw new ArgumentException("Value \"n_max\" must be positive");

            if (points < 0)
                throw new ArgumentException("Value \"points\" must be positive");

            // Function for integration
            double Integrant(double t)
            {
                double alpha = sigma - beta;
                return f(t) * SolvePolynomial(t, maxN) * Math.Exp(-alpha * t);
            }

            // Upper bound of integration
            var optimalT = FindOptimalT(maxN);
            double maxT = optimalT.Key;

            // Integration
            var integralSolver = new IntegralSolver(Integrant);
            double result = integralSolver.Solve(0, maxT, points);
            return result;
        }

        public Dictionary<int, double> TabulateLaguerreTransform(Func<double, double> f, int maxN, int points = 10000)
        {
            if (maxN < 0)
                throw new ArgumentException("Value \"maxN\" must be positive");

            if (points < 0)
                throw new ArgumentException("Value \"points\" must be positive");

            var result = new Dictionary<int, double>();
            for (int n = 0; n < maxN; n++)
            {
                result.Add(n, SolveLaguerreTransform(f, n, points));
            }
            return result;
        }

        public double SolveInverseLaguerreTransform(double[] h, double t)
        {
            double result = 0;
            for (int k = 0; k < h.Length; k++)
            {
                result += h[k] * SolvePolynomial(t, k);
            }
            return result;
        }
    }
}
