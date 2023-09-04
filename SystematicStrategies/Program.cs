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
            DataUtilities dataUtilities = new DataUtilities();
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer(dataUtilities.testParameters);
            ComputationUtilities computationUtilities = new ComputationUtilities(pricer, dataUtilities);
            List<DateTime> dateTimes = dataUtilities.GetDateTimes(dataUtilities.marketData);

            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[0]);
            Portfolio portfolio = new Portfolio(marketDataCurrDate, computationUtilities.GetPrice(dateTimes[0], dataUtilities.maturity));
            PortfolioUtilities portfolioUtilities = new PortfolioUtilities(portfolio, computationUtilities);
            List<PricingLibrary.DataClasses.OutputData> outputDatas = new List<PricingLibrary.DataClasses.OutputData>();

            portfolioUtilities.UpdateCompo(marketDataCurrDate);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                var marketDataPrevDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t - 1]);
                marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                var vt = portfolioUtilities.UpdatePortfolioValue(marketDataPrevDate, marketDataCurrDate);
                if (portfolioUtilities.RebalancingTime(t))
                {
                    portfolioUtilities.UpdateCompo(marketDataCurrDate);
                }
            }
            //return outputDatas;
        }
    }
}
