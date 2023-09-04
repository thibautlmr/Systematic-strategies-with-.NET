using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using PricingLibrary.MarketDataFeed;
using CsvHelper.Configuration;
using CsvHelper;

namespace SystematicStrategies
{
    public class CsvDataReader
    {
        public static List<ShareValue> GetData(string csvPath)
        {
            var marketData = new List<ShareValue>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = csv.GetRecords<ShareValue>();

                foreach (var record in records)
                {
                    var shareValue = new ShareValue
                    {
                        DateOfPrice = record.DateOfPrice,
                        Id = record.Id,
                        Value = record.Value
                    };
                    marketData.Add(shareValue);
                }
            }
            return marketData;
        }
    }
}
