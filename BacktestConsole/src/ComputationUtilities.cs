using PricingLibrary.MarketDataFeed;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BacktestConsole.src
{
    internal class ComputationUtilities
    {
        public PricingLibrary.Computations.Pricer Pricer;
        public DataUtilities DataUtilities;

        public ComputationUtilities(PricingLibrary.Computations.Pricer pricer, DataUtilities dataUtilities)
        {
            Pricer = pricer;
            DataUtilities = dataUtilities;
        }

        public bool RebalancingTime(int t, List<ShareValue> marketDataCurrDate)
        {
            IRebalancingOracleDescription rebalancingOracleType = DataUtilities.TestParameters.RebalancingOracleDescription;
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
                if (marketDataCurrDate[0].DateOfPrice.DayOfWeek == day)
                {
                    return true;
                }
            }
            return false;
        }

        public double GetFreeRate(DateTime date1, DateTime date2)
        {
            return RiskFreeRateProvider.GetRiskFreeRateAccruedValue(date1, date2);
        }

        public double[] GetSpots(DateTime date)
        {
            List<ShareValue> marketDataCurrDate = DataUtilities.GetShareValuesForOneDate(date);
            double[] spots = new double[marketDataCurrDate.Count];
            for (int i = 0; i < marketDataCurrDate.Count; i++)
            {
                spots[i] = marketDataCurrDate[i].Value;
            }
            return spots;
        }

        public double[] GetDeltas(DateTime date, DateTime maturity)
        {
            List<ShareValue> marketDataCurrDate = DataUtilities.GetShareValuesForOneDate(date);
            double[] spots = GetSpots(date);
            return Pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).Deltas;
        }

        public double[] GetDeltaStdDev(DateTime date, DateTime maturity)
        {
            List<ShareValue> marketDataCurrDate = DataUtilities.GetShareValuesForOneDate(date);
            double[] spots = GetSpots(date);
            return Pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).DeltaStdDev;
        }


        public double GetPrice(DateTime currDate, DateTime maturity)
        {
            return Pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(currDate, maturity), GetSpots(currDate)).Price;
        }

        public double GetPriceStdDev(DateTime date, DateTime maturity)
        {
            List<ShareValue> marketDataCurrDate = DataUtilities.GetShareValuesForOneDate(date);
            double[] spots = GetSpots(date);
            return Pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).PriceStdDev;
        }
    }


}
