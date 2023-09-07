using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Computations;
using PricingLibrary.MarketDataFeed;

namespace BacktestLibrary
{
    public class Handler
    {
        public Portfolio Portfolio;
        public ComputationUtilities ComputationUtilities;
        public List<ShareValue> MarketDataPrevDate;
        public List<ShareValue> MarketDataCurrDate;

        public Handler(ComputationUtilities computationUtilities, List<ShareValue> marketDataCurrDate)
        {
            ComputationUtilities = computationUtilities;
            MarketDataPrevDate = marketDataCurrDate;
            MarketDataCurrDate = marketDataCurrDate;
            Portfolio = InitializePortfolioCompo();
        }

        private Portfolio InitializePortfolioCompo()
        {
            double[] deltas = ComputationUtilities.GetDeltas(DataUtilities.GetDateTimes(MarketDataCurrDate)[0], ComputationUtilities.DataUtilities.Maturity);
            Dictionary<String, double> compo = new();
            List<string> ids = ComputationUtilities.DataUtilities.GetIds();
            double temp = GetPortfolioValue();
            for (int i = 0; i < MarketDataCurrDate.Count; i++)
            {
                compo.Add(ids[i], deltas[i]);
                temp -= compo[ids[i]] * MarketDataCurrDate[i].Value;
            }
            return new Portfolio(compo, temp, 1);
        }

        public double GetPortfolioValue()
        {
            if (MarketDataPrevDate == MarketDataCurrDate)
            {
                return ComputationUtilities.GetPrice(DataUtilities.GetDateTimes(ComputationUtilities.DataUtilities.MarketData)[0], ComputationUtilities.DataUtilities.Maturity);
            }
            double portfolioValue = 0;
            for (int i = 0; i < MarketDataCurrDate.Count; i++)
            {
                portfolioValue += Portfolio.Composition[MarketDataCurrDate[i].Id] * MarketDataCurrDate[i].Value;
            }
            DateTime prevDate = MarketDataPrevDate[0].DateOfPrice;
            DateTime currDate = MarketDataCurrDate[0].DateOfPrice;
            Portfolio.FreeRate = ComputationUtilities.GetFreeRate(prevDate, currDate);
            portfolioValue += Portfolio.FreeRate * Portfolio.FreeRateQuantity;
            return portfolioValue;
        }

        public void UpdateCompo()
        {
            DateTime prevDate = MarketDataPrevDate[0].DateOfPrice;
            DateTime currDate = MarketDataCurrDate[0].DateOfPrice;
            double total = GetPortfolioValue();
            for (int i = 0; i < MarketDataCurrDate.Count; i++)
            {
                Portfolio.Composition[MarketDataCurrDate[i].Id] = ComputationUtilities.GetDeltas(currDate, ComputationUtilities.DataUtilities.Maturity)[i];
                total -= Portfolio.Composition[MarketDataCurrDate[i].Id] * MarketDataCurrDate[i].Value;
            }
            Portfolio.FreeRateQuantity = total;
            Portfolio.FreeRate = ComputationUtilities.GetFreeRate(prevDate, currDate);
        }

        public void AddOutputData(List<PricingLibrary.DataClasses.OutputData> outputDatas)
        {
            PricingLibrary.DataClasses.OutputData outputData = new()
            {
                Date = MarketDataCurrDate[0].DateOfPrice,
                Value = GetPortfolioValue()
            };
            outputData.Deltas = ComputationUtilities.GetDeltas(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.DeltasStdDev = ComputationUtilities.GetDeltaStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.Price = ComputationUtilities.GetPrice(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputData.PriceStdDev = ComputationUtilities.GetPriceStdDev(outputData.Date, ComputationUtilities.DataUtilities.Maturity);
            outputDatas.Add(outputData);
        }
    }
}
