using PricingLibrary.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematicStrategies
{
    internal class ProgramUtilities
    {
        public ProgramUtilities() 
        {

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

        public List<DateTime> Get(List<PricingLibrary.MarketDataFeed.ShareValue> shares)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            foreach (ShareValue share in shares)
            {
                if (!dateTimes.Contains(share.DateOfPrice))
                {
                    dateTimes.Add(share.DateOfPrice);
                }
            }
            return dateTimes;
        }

        public List<PricingLibrary.MarketDataFeed.ShareValue> GetShareValuesForOneDate(List<PricingLibrary.MarketDataFeed.ShareValue> shares, DateTime dateTime) 
        {
            List<DateTime> dateTimes = GetDateTimes(shares);
            List<ShareValue> sharesOneDate = new List<PricingLibrary.MarketDataFeed.ShareValue>();
            foreach (ShareValue share in shares)
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
