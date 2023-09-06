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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BacktestConsole.src
{
    internal class DataUtilities
    {
        public List<ShareValue> MarketData;
        public BasketTestParameters TestParameters;
        public DateTime Maturity;
        public Input Input;
        public DataUtilities(Input input)
        {
            Input = input;
            MarketData = GetMarketDataFromCsv(input);
            TestParameters = GetBasketTestParametersFromCsv(input);
            Maturity = TestParameters.BasketOption.Maturity;
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(), new RebalancingOracleDescriptionConverter() }
            };
            return options;
        }

        private static List<ShareValue> GetMarketDataFromCsv(Input input)
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

        private static BasketTestParameters GetBasketTestParametersFromCsv(Input input)
        {
            string jsonPath = input.TestParamsPath;
            using StreamReader r = new(jsonPath);
            string json = r.ReadToEnd();
            var options = GetJsonOptions();
            BasketTestParameters? testParameters = JsonSerializer.Deserialize<BasketTestParameters>(json, options);
            if (testParameters != null)
            {
                return testParameters;
            } else
            {
                return new BasketTestParameters();
            }
        }

        public static string GetJsonFromObject(object obj)
        {
            var options = GetJsonOptions();
            return JsonSerializer.Serialize(obj, options);
        }

        public static bool RebalancingTime(int t, ComputationUtilities computationUtilities, List<ShareValue> marketDataCurrDate)
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

        public static List<DateTime> GetDateTimes(List<ShareValue> shares)
        {
            List<DateTime> dateTimes = new();
            foreach (DateTime date in shares.Select(share => share.DateOfPrice).Where(date => !dateTimes.Contains(date)))
            {
                dateTimes.Add(date);
            }
            return dateTimes;
        }

        public List<ShareValue> GetShareValuesForOneDate(DateTime dateTime)
        {
            List<ShareValue> sharesOneDate = new();
            foreach (ShareValue share in MarketData)
            {
                if (share.DateOfPrice == dateTime)
                {
                    sharesOneDate.Add(share);
                }
            }
            return sharesOneDate;
        }

        public List<string> GetIds()
        {
            string[] shareIds = TestParameters.BasketOption.UnderlyingShareIds;
            List<string> ids = new();
            foreach (string id in shareIds) 
            {
                ids.Add(id);
            }
            return ids;
        }
    }
}
