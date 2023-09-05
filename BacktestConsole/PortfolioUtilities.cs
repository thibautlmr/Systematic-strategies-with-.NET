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
        public Portfolio Portfolio;
        public ComputationUtilities ComputationUtilities;

        public PortfolioUtilities(Portfolio portfolio, ComputationUtilities computationUtilities)
        {
            this.Portfolio = portfolio;
            this.ComputationUtilities = computationUtilities;
        }

        public bool RebalancingTime(int t)
        {
            return true;
        }

        public void UpdatePortfolioValue(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)

        {
            double[] spots = ComputationUtilities.GetSpots(marketDataCurrDate[0].DateOfPrice);
            double total = 0;
            for (int i = 0; i < marketDataPrevDate.Count; i++)
            {
                total += Portfolio.PortfolioMap[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            double freeRate = ComputationUtilities.GetFreeRate(marketDataPrevDate[0].DateOfPrice, marketDataCurrDate[0].DateOfPrice);
            total += freeRate * Portfolio.PortfolioMap["freeRate"];
            Portfolio.PrevPortfolioValue = total;
        }

        public void UpdateCompo(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            double[] spots = ComputationUtilities.GetSpots(marketDataCurrDate[0].DateOfPrice);
            DateTime currDate = marketDataCurrDate.First().DateOfPrice;
            double total = Portfolio.PrevPortfolioValue;
            for (int i = 0; i < marketDataCurrDate.Count; i++)
            {
                Portfolio.PortfolioMap[marketDataCurrDate[i].Id] = ComputationUtilities.GetDeltas(currDate, ComputationUtilities.DataUtilities.Maturity)[i];
                total -= Portfolio.PortfolioMap[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            Portfolio.PortfolioMap["freeRate"] = total;
        }

        public void AddOutputData(List<PricingLibrary.DataClasses.OutputData> outputDatas, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            PricingLibrary.DataClasses.OutputData outputData = new PricingLibrary.DataClasses.OutputData();
            outputData.Date = marketDataCurrDate[0].DateOfPrice;
            outputData.Value = Portfolio.PrevPortfolioValue;
            outputData.Deltas = ComputationUtilities.GetDeltas(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.DeltasStdDev = ComputationUtilities.GetDeltaStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.Price = ComputationUtilities.GetPrice(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.PriceStdDev = ComputationUtilities.GetPriceStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputDatas.Add(outputData);
        }
    }
}
