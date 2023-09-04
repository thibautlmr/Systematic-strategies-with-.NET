using PricingLibrary;
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
        public Dictionary<String, double> portfolioMap;
        public double prevPortfolioValue; 
        public Portfolio(List<PricingLibrary.MarketDataFeed.ShareValue> shares, double p)
        {
            Dictionary<String, double> portfolioMap = new Dictionary<String, double>();
            portfolioMap.Add("freeRate", 0);
            if (shares != null)
            {
                foreach (PricingLibrary.MarketDataFeed.ShareValue share in shares)
                {
                    if (share != null) { portfolioMap.Add(share.Id, 0); }
                }
            }
            this.portfolioMap = portfolioMap;
            this.prevPortfolioValue = p;
        }
    }
}
