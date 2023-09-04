using PricingLibrary.Computations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace SystematicStrategies {
    internal class Portfolio
    {
        Dictionary<String, double> portfolioMap;
        double freeRateQuantity;
        double prevPortfolioValue;
        public DateTime maturity;
        public Portfolio(List<PricingLibrary.MarketDataFeed.ShareValue> shares, double p, DateTime maturity)
        {
            Dictionary<String, double> portfolioMap = new Dictionary<String, double>();
            if (shares != null)
            {
                foreach (PricingLibrary.MarketDataFeed.ShareValue share in shares)
                {
                    if (share != null) { portfolioMap.Add(share.Id, 0); }
                }
            }
            this.portfolioMap = portfolioMap;
            freeRateQuantity = 1;
            prevPortfolioValue = p;
            this.maturity = maturity;
        }

        public double UpdatePortfolioValue(List<PricingLibrary.MarketDataFeed.ShareValue> marketDataPrevDate, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)

        { 
            double[] spots = new double[] { marketDataPrevDate[0].Value };
            // a modifier car un seul actif
            double currPrice = marketDataCurrDate[0].Value;
            double prevPrice = marketDataPrevDate[0].Value;
            prevPortfolioValue = portfolioMap[portfolioMap.Keys.ToList()[0]] * currPrice + PricingLibrary.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(marketDataPrevDate[0].DateOfPrice, marketDataCurrDate[0].DateOfPrice) * freeRateQuantity;
            return prevPortfolioValue;
        }

        public bool RebalancingTime(int t)
        {
            return true;
        }

        public void UpdateCompo(PricingLibrary.Computations.Pricer pricer, List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate)
        {
            double[] spots = new double[] { marketDataCurrDate[0].Value };
            portfolioMap[portfolioMap.Keys.ToList()[0]] = pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(marketDataCurrDate[0].DateOfPrice, maturity), spots).Deltas[0];
            double currPrice = marketDataCurrDate[0].Value;
            freeRateQuantity = prevPortfolioValue - portfolioMap[portfolioMap.Keys.ToList()[0]] * currPrice;
        }
    }
}
