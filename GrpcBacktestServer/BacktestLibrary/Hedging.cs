using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BacktestLibrary;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;

namespace BacktestLibrary
{
    public class Hedging
    {
        public List<OutputData> OutputData;
        public void Run(List<ShareValue> marketData, BasketTestParameters testParameters, string outputFilePath)
        {
            DataUtilities dataUtilities = new(new Input(marketData, testParameters, outputFilePath));
            Pricer pricer = new(dataUtilities.TestParameters);
            ComputationUtilities computationUtilities = new(pricer, dataUtilities);
            List<DateTime> dateTimes = DataUtilities.GetDateTimes(dataUtilities.MarketData);
            var marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[0]);
            Handler handler = new(computationUtilities, marketDataCurrDate);
            List<OutputData> outputDatas = new();

            handler.AddOutputData(outputDatas);

            for (int t = 1; t < dateTimes.Count; t++)
            {
                handler.MarketDataCurrDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                if (DataUtilities.RebalancingTime(t, computationUtilities, handler.MarketDataCurrDate))
                {
                    handler.GetPortfolioValue();
                    handler.UpdateCompo();
                    handler.AddOutputData(outputDatas);
                    handler.MarketDataPrevDate = dataUtilities.GetShareValuesForOneDate(dateTimes[t]);
                }
            }
            File.WriteAllText(dataUtilities.Input.OutputFilePath, DataUtilities.GetJsonFromObject(outputDatas));
            this.OutputData = outputDatas;
        }
    }
}
