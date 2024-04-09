using System.Numerics;

namespace Laguerre
{
    class App
    {
        public const double BETA = 2.0;
        public const double SIGMA = 4.0;
        public const int N = 5;
        public static double GAUSSIAN_MU = 0;
        public static double GAUSSIAN_LAMBDA = 1;
        public static Func<double, double> F = (double x) => Math.Pow(x, 2);
        public static Func<double, double> NORMAL_GAUSSIAN_DISTRIBUTION =
            (double x) =>
            Math.Exp(
                -Math.Pow(x - GAUSSIAN_MU, 2) /
                (2 * Math.Pow(GAUSSIAN_LAMBDA, 2))
            ) / (GAUSSIAN_LAMBDA * Math.Sqrt(2 * Math.PI));

        static void Main()
        {
            LaguerreSolver solver = new LaguerreSolver(BETA, SIGMA);

            Console.WriteLine("\nLaguerre polynomials tabulation:\n");
            var polynomialTabulation = solver.TabulatePolynomial(N, 10, 0.1);
            PrintTabulation(polynomialTabulation, new List<string> { "t", "L(t)" });

            Console.WriteLine("\n\n\nExperiment:\n");
            var experimentResults = solver.FindOptimalT(N);
            Console.WriteLine($"Optimal t: {experimentResults.Key}\n");
            PrintTabulation(experimentResults.Value, new List<string> { "t", "L(t)" });

            Console.WriteLine("\n\n\nTransformation:\n");
            var transformationResults = solver.TabulateLaguerreTransform(F, N);
            PrintTabulation(transformationResults, new List<string> { "n", "L_n" });

            Console.WriteLine("\n\n\nInverse transformation:\n");
            double[] h = transformationResults.Values.ToArray();
            var inverseTransformationResult = solver.SolveInverseLaguerreTransform(h, 10);
            Console.WriteLine(inverseTransformationResult);

            Console.WriteLine("\n\n\nGaussian Transformation:\n");
            var gaussianTransformationResults = solver.TabulateLaguerreTransform(NORMAL_GAUSSIAN_DISTRIBUTION, N);
            PrintTabulation(gaussianTransformationResults, new List<string> { "n", "L_n" });

            Console.WriteLine("\n\n\nGaussian inverse transformation:\n");
            double[] gaussianH = gaussianTransformationResults.Values.ToArray();
            var gaussianInverseTransformationResult = solver.SolveInverseLaguerreTransform(gaussianH, 10);
            Console.WriteLine(gaussianInverseTransformationResult);
        }

        static void PrintTabulation<T, K>(Dictionary<T, K> tabulation, List<string> headers, int columnWidth = 8) where T : INumber<T> where K : INumber<K>
        {
            static string NormalizeWidth<V>(V value, int width)
            {
                if (value == null)
                    return new string(' ', width);

                var str = value.ToString();

                if (string.IsNullOrEmpty(str))
                    return new string(' ', width);

                if (str.Length > width)
                    return str.Substring(0, width);

                return str.PadLeft(width);

            }

            headers = headers.Select(header => NormalizeWidth(header, columnWidth)).ToList();
            var headerStr = "# " + string.Join(" # ", headers) + " #";
            var headerDividerString = new string('=', headerStr.Length);
            var dividerStr = new string('-', headerStr.Length);
            Console.WriteLine(headerDividerString);
            Console.WriteLine(headerStr);
            Console.WriteLine(headerDividerString);

            foreach (var pair in tabulation)
            {
                var firstValueStr = NormalizeWidth(pair.Key, columnWidth);
                var secondValueStr = NormalizeWidth(pair.Value, columnWidth);

                var rowStr = "| " + firstValueStr + " | " + secondValueStr + " |";
                Console.WriteLine(rowStr);
                Console.WriteLine(dividerStr);

            }
        }
    }
}
