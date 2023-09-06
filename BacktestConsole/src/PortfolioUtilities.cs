using PricingLibrary.Computations;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BacktestConsole.src
{
    internal class PortfolioUtilities
    {
        public Portfolio Portfolio;
        public ComputationUtilities ComputationUtilities;

        public PortfolioUtilities(Portfolio portfolio, ComputationUtilities computationUtilities)
        {
            Portfolio = portfolio;
            ComputationUtilities = computationUtilities;
        }

        public double GetPortfolioValue(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            if (marketDataPrevDate == marketDataCurrDate) 
            {
                return ComputationUtilities.GetPrice(ComputationUtilities.DataUtilities.GetDateTimes(ComputationUtilities.DataUtilities.MarketData).First(), 
                    ComputationUtilities.DataUtilities.Maturity);
            }
            DateTime currDate = marketDataCurrDate.First().DateOfPrice;
            DateTime prevDate = marketDataPrevDate.First().DateOfPrice;
            double[] spots = ComputationUtilities.GetSpots(currDate);
            double portfolioValue = 0;
            for (int i = 0; i < marketDataPrevDate.Count; i++)
            {
                portfolioValue += Portfolio.Composition[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            double freeRate = ComputationUtilities.GetFreeRate(prevDate, currDate);
            portfolioValue += freeRate * Portfolio.Composition["freeRate"];
            return portfolioValue;
        }

        public void UpdateCompo(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            DateTime currDate = marketDataCurrDate.First().DateOfPrice;
            double[] spots = ComputationUtilities.GetSpots(currDate);
            double total = GetPortfolioValue(marketDataPrevDate, marketDataCurrDate);
            for (int i = 0; i < marketDataCurrDate.Count; i++)
            {
                Portfolio.Composition[marketDataCurrDate[i].Id] = ComputationUtilities.GetDeltas(currDate, ComputationUtilities.DataUtilities.Maturity)[i];
                total -= Portfolio.Composition[marketDataCurrDate[i].Id] * marketDataCurrDate[i].Value;
            }
            Portfolio.Composition["freeRate"] = total;
        }

        public void AddOutputData(List<PricingLibrary.DataClasses.OutputData> outputDatas, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, 
            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            PricingLibrary.DataClasses.OutputData outputData = new PricingLibrary.DataClasses.OutputData();
            outputData.Date = marketDataCurrDate.First().DateOfPrice;
            outputData.Value = GetPortfolioValue(marketDataPrevDate, marketDataCurrDate);
            outputData.Deltas = ComputationUtilities.GetDeltas(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.DeltasStdDev = ComputationUtilities.GetDeltaStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.Price = ComputationUtilities.GetPrice(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.PriceStdDev = ComputationUtilities.GetPriceStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputDatas.Add(outputData);
        }
    }
}
