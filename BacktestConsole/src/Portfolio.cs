using PricingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BacktestConsole.src
{
    internal class Portfolio
    {
        public Dictionary<string, double> Composition;
        public Portfolio(List<PricingLibrary.MarketDataFeed.ShareValue> shares)
        {
            Dictionary<string, double> composition = new Dictionary<string, double>();
            composition.Add("freeRate", 0);
            if (shares != null)
            {
                foreach (PricingLibrary.MarketDataFeed.ShareValue share in shares)
                {
                    if (share != null) { composition.Add(share.Id, 0); }
                }
            }
            Composition = composition;
        }
    }
}
