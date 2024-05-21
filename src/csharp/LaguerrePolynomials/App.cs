using System.Numerics;
using System.Text.Json;

namespace Laguerre
{
    class App
    {
        public const double BETA = 2.0;
        public const double SIGMA = 4.0;
        public const int N = 5;

        public static Func<double, double> F_1 = (double t) => Math.Cos(t + Math.Exp(t) / 2);
        public static Func<double, double> F_2 = (double t) => Math.Sin(t) * Math.Cos(t);
        public static Func<double, double> F_3 = (double t) => Math.Cos(Math.PI - t) * t / 2;
        public static Func<double, double> F_4 = (double t) => t != 0 ? Math.Cos(2 * t + Math.PI) * t : 0;

        public static double GAUSSIAN_MU = 0;
        public static double GAUSSIAN_LAMBDA = 1;
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
            var transformationResults = solver.TabulateLaguerreTransform(F_1, N);
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

            SavePolynomialsTabulationToFile(20, 4, 0.1);

            SaveTransformationToFile(F_1, "f1", 20, 2, 0.1, 100000);
            SaveTransformationToFile(F_2, "f2", 20, 10, 0.1);
            SaveTransformationToFile(F_3, "f3", 20, 10, 0.1);
            SaveTransformationToFile(F_4, "f4", 20, 10, 0.1, 100000);

            SaveTransformationToFile(NORMAL_GAUSSIAN_DISTRIBUTION, "normal_gaussian", 20, 2, 0.1);
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

        static void SavePolynomialsTabulationToFile(int maxN, double maxT, double tStep)
        {
            LaguerreSolver solver = new LaguerreSolver(BETA, SIGMA);

            if (Directory.Exists("output/polynomials"))
                Directory.Delete("output/polynomials", true);

            Directory.CreateDirectory("output/polynomials");

            for (int n = 0; n <= maxN; n++)
            {
                var tabulation = solver.TabulatePolynomial(n, maxT, tStep);
                using (StreamWriter file = File.CreateText($"output/polynomials/polynomials_n{n}.csv"))
                {
                    file.WriteLine("t,l");
                    foreach (var pair in tabulation)
                    {
                        file.WriteLine($"{pair.Key},{pair.Value}");
                    }
                }
            }
        }

        static void SaveTransformationToFile(
            Func<double, double> f,
            string functionName,
            int maxN,
            double maxT,
            double tStep,
            int points = 10000
        )
        {
            LaguerreSolver solver = new LaguerreSolver(BETA, SIGMA);

            if (Directory.Exists($"output/transformations/{functionName}"))
                Directory.Delete($"output/transformations/{functionName}", true);

            Directory.CreateDirectory($"output/transformations/{functionName}");

            var transformTabulation = solver.TabulateLaguerreTransform(f, maxN, points);
            var h = transformTabulation.Values.ToArray();

            var initialTabulation = new Dictionary<double, double>();
            var inverseTransformTabulation = new Dictionary<double, double>();

            for (double t = 0; t <= maxT; t += tStep)
            {
                initialTabulation.Add(t, f(t));
                inverseTransformTabulation.Add(t, solver.SolveInverseLaguerreTransform(h, t));
            }

            using (StreamWriter file = File.CreateText($"output/transformations/{functionName}/initial.csv"))
            {
                file.WriteLine("t,f");
                foreach (var pair in initialTabulation)
                {
                    file.WriteLine($"{pair.Key},{pair.Value}");
                }
            }

            using (StreamWriter file = File.CreateText($"output/transformations/{functionName}/transform.csv"))
            {
                file.WriteLine("n,l");
                foreach (var pair in transformTabulation)
                {
                    file.WriteLine($"{pair.Key},{pair.Value}");
                }
            }

            using (StreamWriter file = File.CreateText($"output/transformations/{functionName}/inverse.csv"))
            {
                file.WriteLine("t,h");
                foreach (var pair in inverseTransformTabulation)
                {
                    file.WriteLine($"{pair.Key},{pair.Value}");
                }
            }

            using (StreamWriter file = File.CreateText($"output/transformations/{functionName}/metadata.json"))
            {
                var metadata = new
                {
                    functionName,
                    maxN,
                    maxT,
                    tStep,
                    points
                };
                file.WriteLine(JsonSerializer.Serialize(metadata));
            }
        }
    }
}
