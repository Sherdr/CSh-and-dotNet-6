using System.Diagnostics;

namespace LinqInParallel {
    internal class Program {
        static void Main(string[] args) {
            Stopwatch watch = new();
            Console.WriteLine("Press ENTER to start.");
            Console.ReadLine();
            watch.Start();
            int max = 45;
            IEnumerable<int> numbers = Enumerable.Range(1, max);
            Console.WriteLine($"Calculating Fibonacci sequence up to {max}. Please wait...");
            int[] fibonacciNumbers = numbers.AsParallel().
                Select(number => Fibonacci(number)).
                OrderBy(number => number).ToArray();
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds:#,##0} elapsed ms.");
            Console.WriteLine("Results:");
            foreach(var number in fibonacciNumbers) {
                Console.Write($"{number} ");
            }
        }

        static int Fibonacci(int term) {
            switch (term) {
                case 1: return 0;
                case 2: return 1;
                default: return (Fibonacci(term - 1) + Fibonacci(term - 2));
            }
        }
    }
}
