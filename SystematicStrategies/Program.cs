// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ScottPlot;

namespace SystematicStrategies
{
    class Program
    {
        static void Main()
        {
            var marketData = PricingLibrary.Utilities.SampleMarketData.Sample().ToList();
            var testParameters = PricingLibrary.Utilities.SampleTestParameters.Sample();
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
            plt.SaveFig("C:\\Users\\lamur\\Documents\\3aif\\Systematic-strategies-with-.NET\\SystematicStrategies\\plot.png");

        }
    }

}
