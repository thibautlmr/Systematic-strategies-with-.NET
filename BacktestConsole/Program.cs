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


namespace SystematicStrategies
{
    class Program
    {
        static void Main(String[] args)
        { 
            Input input = new Input(args);
            DataUtilities dataUtilities = new DataUtilities(input);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer(dataUtilities.TestParameters);
            ComputationUtilities computationUtilities = new ComputationUtilities(pricer, dataUtilities);
            List<DateTime> dateTimes = dataUtilities.GetDateTimes(dataUtilities.MarketData);

            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[0]);
            Portfolio portfolio = new Portfolio(marketDataCurrDate, computationUtilities.GetPrice(dateTimes[0], dataUtilities.Maturity));
            PortfolioUtilities portfolioUtilities = new PortfolioUtilities(portfolio, computationUtilities);
            List<PricingLibrary.DataClasses.OutputData> outputDatas = new List<PricingLibrary.DataClasses.OutputData>();

            portfolioUtilities.UpdateCompo(marketDataCurrDate);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                var marketDataPrevDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t - 1]);
                marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                portfolioUtilities.UpdatePortfolioValue(marketDataPrevDate, marketDataCurrDate);
                if (portfolioUtilities.RebalancingTime(t))
                {
                    portfolioUtilities.UpdateCompo(marketDataCurrDate);
                }
                portfolioUtilities.AddOutputData(outputDatas, marketDataCurrDate);
            }
            System.IO.File.WriteAllText(input.OutputFilePath, dataUtilities.GetJsonFromObject(outputDatas));
        }
    }
}
