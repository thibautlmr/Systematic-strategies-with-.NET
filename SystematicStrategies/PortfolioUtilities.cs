using PricingLibrary.Computations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematicStrategies
{
    internal class PortfolioUtilities
    {
        public Portfolio portfolio;
        public ComputationUtilities computationUtilities;

        public PortfolioUtilities(Portfolio portfolio, ComputationUtilities computationUtilities)
        {
            this.portfolio = portfolio;
            this.computationUtilities = computationUtilities;
        }

        public bool RebalancingTime(int t)
        {
            return true;
        }

        public double UpdatePortfolioValue(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)

        {
            double[] spots = computationUtilities.getSpots(marketDataCurrDate[0].DateOfPrice);
            double total = 0;
            for (int i = 0; i < marketDataPrevDate.Count; i++)
            {
                total += portfolio.portfolioMap[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            double freeRate = computationUtilities.GetFreeRate(marketDataPrevDate[0].DateOfPrice, marketDataCurrDate[0].DateOfPrice);
            total += freeRate * portfolio.portfolioMap["freeRate"];
            portfolio.prevPortfolioValue = total;
            return portfolio.prevPortfolioValue;
        }

        public void UpdateCompo(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            double[] spots = computationUtilities.getSpots(marketDataCurrDate[0].DateOfPrice);
            DateTime currDate = marketDataCurrDate.First().DateOfPrice;
            double total = portfolio.prevPortfolioValue;
            for (int i = 0; i < marketDataCurrDate.Count; i++)
            {
                portfolio.portfolioMap[marketDataCurrDate[i].Id] = computationUtilities.GetDeltas(currDate, computationUtilities.dataUtilities.maturity)[i];
                total -= portfolio.portfolioMap[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            portfolio.portfolioMap["freeRate"] = total;
        }

        public void addOutputData(List<PricingLibrary.DataClasses.OutputData> outputDatas, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            PricingLibrary.DataClasses.OutputData outputData = new PricingLibrary.DataClasses.OutputData();
            outputData.Date = marketDataCurrDate[0].DateOfPrice;
            outputData.Value = portfolio.prevPortfolioValue;
            outputData.Deltas = computationUtilities.GetDeltas(outputData.Date, computationUtilities.dataUtilities.maturity);
            outputData.DeltasStdDev = computationUtilities.GetDeltaStdDev(outputData.Date, computationUtilities.dataUtilities.maturity);
            outputData.Price = computationUtilities.GetPrice(outputData.Date, computationUtilities.dataUtilities.maturity);
            outputData.PriceStdDev = computationUtilities.GetPriceStdDev(outputData.Date, computationUtilities.dataUtilities.maturity);
            outputDatas.Add(outputData);
        }
    }
}
