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


namespace SystematicStrategies
{
    class Program
    {
        static void Main()
        {
            string csvPath = "C:\\Users\\Erwan Izenic\\OneDrive\\Documents\\COURS_3A\\Systematic-strategies-with-.NET\\SystematicStrategies\\Resources\\TestData\\Test_1_2\\data_1_2.csv";
            var marketData = new List<ShareValue>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = csv.GetRecords<ShareValue>();

                foreach (var record in records)
                {
                    var shareValue = new ShareValue
                    {
                        DateOfPrice = record.DateOfPrice,
                        Id = record.Id,
                        Value = record.Value
                    };
                    marketData.Add(shareValue);
                }
            }
            string jsonPath = "C:\\Users\\Erwan Izenic\\OneDrive\\Documents\\COURS_3A\\Systematic-strategies-with-.NET\\SystematicStrategies\\Resources\\TestData\\Test_1_2\\params_1_2.json";
            BasketTestParameters testParameters = null;
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string json = r.ReadToEnd();

                var model = System.Text.Json.JsonSerializer.Deserialize<BasketTestParametersModel>(json);

                testParameters = new BasketTestParameters
                {
                    PricingParams = new BasketPricingParameters
                    {
                        Volatilities = model.pricingParams.volatilities,
                        Correlations = model.pricingParams.correlations
                    },
                    BasketOption = new Basket
                    {
                        Strike = model.basketOption.strike,
                        Maturity = model.basketOption.maturity,
                        UnderlyingShareIds = model.basketOption.underlyingShareIds,
                        Weights = model.basketOption.weights
                    },
                    RebalancingOracleDescription = new PricingLibrary.RebalancingOracleDescriptions.RegularOracleDescription
                    {
                        Period = model.rebalancingOracleDescription.period
                    }
                };
            }


            //var testParameters = PricingLibrary.Utilities.SampleTestParameters.Sample();
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer(testParameters);
            var p = pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(marketData[0].DateOfPrice, marketData.Last().DateOfPrice), new double[]{ marketData[0].Value }).Price;
            List<double> portfolioValues = new List<double>() { p};
            ProgramUtilities programUtilities = new ProgramUtilities();
            List<DateTime> dateTimes = programUtilities.GetDateTimes(marketData);
            var marketDataCurrDate = programUtilities.GetShareValuesForOneDate(marketData, dateTimes[0]);
            Portfolio portfolio = new Portfolio(marketDataCurrDate, p, testParameters.BasketOption.Maturity);
            portfolio.UpdateCompo(pricer, marketDataCurrDate);
            List<double> optionPrices = new List<double>();
            optionPrices.Add(p);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                var marketDataPrevDate = programUtilities.GetShareValuesForOneDate(marketData, dateTimes[t-1]);
                marketDataCurrDate = programUtilities.GetShareValuesForOneDate(marketData, dateTimes[t]);
                var vt = portfolio.UpdatePortfolioValue(marketDataPrevDate, marketDataCurrDate);
                optionPrices.Add(pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(marketDataCurrDate[0].DateOfPrice, portfolio.maturity), new double[] { marketDataCurrDate[0].Value }).Price);
                portfolioValues.Add(vt);
                if (portfolio.RebalancingTime(t))
                {
                    portfolio.UpdateCompo(pricer, marketDataCurrDate);
                }
            }
            var dates = Enumerable.Range(0, dateTimes.Count()).Select(x => (double)x).ToArray(); 
            var plt = new ScottPlot.Plot(1200, 1200);
            plt.AddScatter(dates, portfolioValues.ToArray(), label:"Hedging", color:Color.Blue);
            plt.AddScatter(dates, optionPrices.ToArray(), label:"Theorical prices of the option", color:Color.Red);
            plt.SaveFig("C:\\Users\\Erwan Izenic\\OneDrive\\Documents\\COURS_3A\\Systematic-strategies-with-.NET\\SystematicStrategies\\plot.png");

        }
    }

}
