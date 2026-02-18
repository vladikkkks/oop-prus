using System;
using System.Collections.Generic;

namespace lab24
{
    public interface INumericOperationStrategy
    {
        string Name { get; }
        double Execute(double value);
    }

    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Квадрат";
        public double Execute(double value) => value * value;
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Куб";
        public double Execute(double value) => Math.Pow(value, 3);
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Квадратний корінь";
        public double Execute(double value) => Math.Sqrt(value);
    }

    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;
        public NumericProcessor(INumericOperationStrategy strategy) => _strategy = strategy;
        public void SetStrategy(INumericOperationStrategy strategy) => _strategy = strategy;
        public double Process(double input) => _strategy.Execute(input);
        public string GetCurrentStrategyName() => _strategy.Name;
    }

    public class ResultPublisher
    {
        public event Action<double, string> ResultCalculated;
        public void PublishResult(double result, string operationName) => ResultCalculated?.Invoke(result, operationName);
    }

    public class ConsoleLoggerObserver
    {
        public void OnResultCalculated(double result, string opName) => 
            Console.WriteLine($"[Console] {opName}: {result:F2}");
    }

    public class HistoryLoggerObserver
    {
        public List<string> History { get; } = new List<string>();
        public void OnResultCalculated(double result, string opName) => 
            History.Add($"{DateTime.Now:HH:mm:ss} - {opName}: {result:F2}");
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;
        public ThresholdNotifierObserver(double threshold) => _threshold = threshold;
        public void OnResultCalculated(double result, string opName)
        {
            if (result > _threshold) Console.WriteLine($"[УВАГА] Перевищено поріг {_threshold}: {result:F2}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var publisher = new ResultPublisher();
            var processor = new NumericProcessor(new SquareOperationStrategy());

            var consoleObs = new ConsoleLoggerObserver();
            var historyObs = new HistoryLoggerObserver();
            var thresholdObs = new ThresholdNotifierObserver(50.0);

            publisher.ResultCalculated += consoleObs.OnResultCalculated;
            publisher.ResultCalculated += historyObs.OnResultCalculated;
            publisher.ResultCalculated += thresholdObs.OnResultCalculated;

            double[] data = { 6, 4, 144 };
            
            double res1 = processor.Process(data[0]);
            publisher.PublishResult(res1, processor.GetCurrentStrategyName());

            processor.SetStrategy(new CubeOperationStrategy());
            double res2 = processor.Process(data[1]);
            publisher.PublishResult(res2, processor.GetCurrentStrategyName());

            processor.SetStrategy(new SquareRootOperationStrategy());
            double res3 = processor.Process(data[2]);
            publisher.PublishResult(res3, processor.GetCurrentStrategyName());

            Console.WriteLine("\n--- Історія операцій ---");
            historyObs.History.ForEach(Console.WriteLine);
        }
    }
}