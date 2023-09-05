using CsvHelper.Configuration;
using CsvHelper;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PricingLibrary.RebalancingOracleDescriptions;
using System.Text.Json.Serialization;

namespace SystematicStrategies
{
    internal class DataUtilities
    {
        public List<PricingLibrary.MarketDataFeed.ShareValue> MarketData;
        public PricingLibrary.DataClasses.BasketTestParameters TestParameters;
        public DateTime Maturity;
        public DataUtilities(Input input)
        { 
            this.MarketData = GetMarketDataFromCsv(input);
            this.TestParameters = GetBasketTestParametersFromCsv(input);
            this.Maturity = TestParameters.BasketOption.Maturity;
        }

        private JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(), new RebalancingOracleDescriptionConverter() }
            };
            return options;
        }

        private List<ShareValue> GetMarketDataFromCsv(Input input)
        {
            string csvPath = input.MarketDataPath;
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

        private PricingLibrary.DataClasses.BasketTestParameters GetBasketTestParametersFromCsv(Input input)
        {
            BasketTestParameters testParameters;
            string jsonPath = input.TestParamsPath;
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string json = r.ReadToEnd();
                var options = GetJsonOptions();
                testParameters = JsonSerializer.Deserialize<BasketTestParameters>(json, options);
            }
            return testParameters;
        }

        public String GetJsonFromObject(Object obj)
        {
            var options = GetJsonOptions();
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
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
            List<DateTime> dateTimes = GetDateTimes(MarketData);
            List<ShareValue> sharesOneDate = new List<PricingLibrary.MarketDataFeed.ShareValue>();
            foreach (ShareValue share in MarketData)
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
