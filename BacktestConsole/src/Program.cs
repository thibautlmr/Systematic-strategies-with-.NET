// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MathNet.Numerics.Distributions;
using ScottPlot;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.Computations;
using Newtonsoft.Json;
using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using Newtonsoft.Json.Linq;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System.Net;


namespace BacktestConsole.src
{
    internal static class Program
    {
        private static void Main(string[] args)
        { 
            DataUtilities dataUtilities = new(new Input(args));
            Pricer pricer = new(dataUtilities.TestParameters);
            ComputationUtilities computationUtilities = new(pricer, dataUtilities);
            List<DateTime> dateTimes = DataUtilities.GetDateTimes(dataUtilities.MarketData);
            var marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[0]);
            Handler handler = new(computationUtilities, marketDataCurrDate);
            List<OutputData> outputDatas = new();

            handler.AddOutputData(outputDatas);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                handler.MarketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                if (DataUtilities.RebalancingTime(t, computationUtilities, handler.MarketDataCurrDate))
                {
                    handler.GetPortfolioValue();
                    handler.UpdateCompo();
                    handler.AddOutputData(outputDatas);
                    handler.MarketDataPrevDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                }
            }
            File.WriteAllText(dataUtilities.Input.OutputFilePath, DataUtilities.GetJsonFromObject(outputDatas));
        }
    }
}
