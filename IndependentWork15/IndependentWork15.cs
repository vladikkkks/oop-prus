using System;
using System.Collections.Generic;

namespace SOLID_Analysis_Lab
{
    public interface IReportSaver
    {
        void Save(string data);
    }

    public class FileSaver : IReportSaver
    {
        public void Save(string data)
        {
            Console.WriteLine($"Saved to File: {data}");
        }
    }

    public class DatabaseSaver : IReportSaver
    {
        public void Save(string data)
        {
            Console.WriteLine($"Saved to Database: {data}");
        }
    }

    public class DataProcessor
    {
        public string Process()
        {
            return "Processed Data Content";
        }
    }

    public class ReportManager
    {
        private readonly IReportSaver _saver;
        private readonly DataProcessor _processor;

        public ReportManager(IReportSaver saver, DataProcessor processor)
        {
            _saver = saver;
            _processor = processor;
        }

        public void Execute()
        {
            string data = _processor.Process();
            _saver.Save(data);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new DataProcessor();
            var saver = new DatabaseSaver();
            
            var manager = new ReportManager(saver, processor);
            manager.Execute();
        }
    }
}