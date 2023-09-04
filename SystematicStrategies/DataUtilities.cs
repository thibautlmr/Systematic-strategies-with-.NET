using PricingLibrary.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematicStrategies
{
    internal class DataUtilities
    {
        public List<PricingLibrary.MarketDataFeed.ShareValue> marketData;
        public PricingLibrary.DataClasses.BasketTestParameters testParameters;
        public DateTime maturity;
        public DataUtilities()
        { 
            // à modifier
            this.marketData = PricingLibrary.Utilities.SampleMarketData.Sample().ToList();
            this.testParameters = PricingLibrary.Utilities.SampleTestParameters.Sample();
            this.maturity = testParameters.BasketOption.Maturity;
        }

        public List<DateTime> GetDateTimes(List<PricingLibrary.MarketDataFeed.ShareValue> shares)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            foreach(ShareValue share in shares)
            {
                if (!dateTimes.Contains(share.DateOfPrice))
                {
                    dateTimes.Add(share.DateOfPrice);
                }
            }
            return dateTimes;
        }

        public List<PricingLibrary.MarketDataFeed.ShareValue> GetShareValuesForOneDate(DateTime dateTime) 
        {
            List<DateTime> dateTimes = GetDateTimes(marketData);
            List<ShareValue> sharesOneDate = new List<PricingLibrary.MarketDataFeed.ShareValue>();
            foreach (ShareValue share in marketData)
            {
                if (share.DateOfPrice == dateTime)
                {
                    sharesOneDate.Add(share);
                }
            }
            return sharesOneDate;
        }
    }
}
