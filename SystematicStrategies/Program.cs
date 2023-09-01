// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SystematicStrategies
{
    class Program
    {
        static void Main()
        {
            var marketData = PricingLibrary.Utilities.SampleMarketData.Sample();
            var testParameters = PricingLibrary.Utilities.SampleTestParameters.Sample();
            List<PricingLibrary.MarketDataFeed.ShareValue> sharesValues = marketData.ToList();

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer(testParameters);
            var p = pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(sharesValues[0].DateOfPrice, sharesValues.Last().DateOfPrice), new double[]{ sharesValues[0].Value }).Price;
            List<double> portfolioValues = new List<double>() { p};

            List<PricingLibrary.MarketDataFeed.ShareValue> shares = new List<PricingLibrary.MarketDataFeed.ShareValue>();
            shares.Add(marketData.FirstOrDefault());
            Portfolio portfolio = new Portfolio(shares);

            var vt1 = portfolio.updatePortfolioValue(pricer, 1, sharesValues, p);

            Console.WriteLine("Hello, World!");
        }
    }

}
