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

namespace BacktestConsole.src
{
    internal class DataUtilities
    {
        public List<ShareValue> MarketData;
        public BasketTestParameters TestParameters;
        public DateTime Maturity;
        public DataUtilities(Input input)
        {
            MarketData = GetMarketDataFromCsv(input);
            TestParameters = GetBasketTestParametersFromCsv(input);
            Maturity = TestParameters.BasketOption.Maturity;
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

        private BasketTestParameters GetBasketTestParametersFromCsv(Input input)
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

        public string GetJsonFromObject(object obj)
        {
            var options = GetJsonOptions();
            return JsonSerializer.Serialize(obj, options);
        }

        public bool RebalancingTime(int t, ComputationUtilities computationUtilities, List<ShareValue> marketDataCurrDate)
        {
            IRebalancingOracleDescription rebalancingOracleType = computationUtilities.DataUtilities.TestParameters.RebalancingOracleDescription;
            if (rebalancingOracleType.Type == RebalancingOracleType.Regular)
            {
                int period = ((RegularOracleDescription)rebalancingOracleType).Period;
                if (t % period == 0)
                {
                    return true;
                }
            }
            else
            {
                DayOfWeek day = ((WeeklyOracleDescription)rebalancingOracleType).RebalancingDay;
                if (marketDataCurrDate[t].DateOfPrice.DayOfWeek == day)
                {
                    return true;
                }
            }
            return false;
        }

        public List<DateTime> GetDateTimes(List<ShareValue> shares)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            for (int i = 0; i < shares.Count; i++)
            {
                if (!dateTimes.Contains(shares[i].DateOfPrice))
                {
                    dateTimes.Add(shares[i].DateOfPrice);
                }
            }
            foreach (ShareValue share in shares)
            {
                if (!dateTimes.Contains(share.DateOfPrice))
                {
                    dateTimes.Add(share.DateOfPrice);
                }
            }
            return dateTimes;
        }

        public List<ShareValue> GetShareValuesForOneDate(DateTime dateTime)
        {
            List<DateTime> dateTimes = GetDateTimes(MarketData);
            List<ShareValue> sharesOneDate = new List<ShareValue>();
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
