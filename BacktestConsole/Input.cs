using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematicStrategies
{
    internal class Input
    {
        public String TestParamsPath;
        public String MarketDataPath;
        public String OutputFilePath;
        public Input(String[] args)
        {
            if (args.Length != 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: 3 parameters are required");
                Console.ResetColor();
                throw new ArgumentException("Error: 3 parameters are required");
            }
            else
            {
                if (args[0].EndsWith(".json"))
                {
                    this.TestParamsPath = args[0];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: the test-params file should be a JSON file");
                    Console.ResetColor();
                    throw new ArgumentException("Error: the test-params file should be a JSON file");
                }
                if (args[1].EndsWith(".csv"))
                {
                    this.MarketDataPath = args[1];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: the market-data file should be a CSV file");
                    Console.ResetColor();
                    throw new ArgumentException("Error: the market-data file should be a CSV file");
                }
                if (args[2].EndsWith(".json"))
                {
                    this.OutputFilePath = args[2];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: the output file should be a JSON file");
                    Console.ResetColor();
                    throw new ArgumentException("Error: the output file should be a JSON file");
                }
            }
        }
    }
}
