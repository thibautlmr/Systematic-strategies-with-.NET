using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace SystematicStrategies {
    internal class Portfolio
    {
        Dictionary<PricingLibrary.MarketDataFeed.ShareValue, float> portfolioMap;
        float previousDelta;
        float previousFreeRateQuantity;
        public Portfolio(List<PricingLibrary.MarketDataFeed.ShareValue> shares)
        {
            Dictionary<PricingLibrary.MarketDataFeed.ShareValue, float> portfolioMap = new Dictionary<PricingLibrary.MarketDataFeed.ShareValue, float>();
            if (shares != null)
            {
                foreach (PricingLibrary.MarketDataFeed.ShareValue share in shares)
                {
                    if (share != null) { portfolioMap.Add(share, 1); }
                }
            }
            this.portfolioMap = portfolioMap;
        }

        public double updatePortfolioValue(PricingLibrary.Computations.Pricer pricer, int t, List<PricingLibrary.MarketDataFeed.ShareValue> marketData, double prevPortfolioValue) 
        {
            DateTime prevDate = marketData[t - 1].DateOfPrice;
            DateTime currDate = marketData[t].DateOfPrice;

            double[] spots = new double[] { marketData[t-1].Value };
            double prevDelta = pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(prevDate, marketData.Last().DateOfPrice), spots).Deltas[0];
            double currPrice = marketData[t].Value;
            double prevPrice = marketData[t - 1].Value;
            double freeRate = PricingLibrary.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(prevDate, currDate);
            return prevDelta * currPrice + freeRate * (prevPortfolioValue - prevDelta * prevPrice);
        }
    }
}
