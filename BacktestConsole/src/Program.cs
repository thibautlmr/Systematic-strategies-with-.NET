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
using CsvHelper.Configuration;
using CsvHelper;
using MathNet.Numerics.Distributions;
using ScottPlot;
using PricingLibrary.MarketDataFeed;
using Newtonsoft.Json;
using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using Newtonsoft.Json.Linq;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System.Net;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace BacktestConsole.src
{
    class Program
    {
        static void Main(string[] args)
        {
            Input input = new Input(args);
            DataUtilities dataUtilities = new DataUtilities(input);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer(dataUtilities.TestParameters);
            ComputationUtilities computationUtilities = new ComputationUtilities(pricer, dataUtilities);
            List<DateTime> dateTimes = dataUtilities.GetDateTimes(dataUtilities.MarketData);

            List<ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[0]);
            List<ShareValue> marketDataPrevDate = marketDataCurrDate;
            Portfolio portfolio = new Portfolio(marketDataCurrDate);
            PortfolioUtilities portfolioUtilities = new PortfolioUtilities(portfolio, computationUtilities);
            List<OutputData> outputDatas = new List<OutputData>();

            portfolioUtilities.UpdateCompo(marketDataPrevDate, marketDataCurrDate);
            portfolioUtilities.AddOutputData(outputDatas, marketDataPrevDate, marketDataCurrDate);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                if (dataUtilities.RebalancingTime(t, computationUtilities, marketDataCurrDate))
                {
                    portfolioUtilities.GetPortfolioValue(marketDataPrevDate, marketDataCurrDate);
                    portfolioUtilities.UpdateCompo(marketDataPrevDate, marketDataCurrDate);
                    portfolioUtilities.AddOutputData(outputDatas, marketDataPrevDate, marketDataCurrDate);
                    marketDataPrevDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                }
            }
            File.WriteAllText(input.OutputFilePath, dataUtilities.GetJsonFromObject(outputDatas));
        }
    }
}
