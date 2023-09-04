using PricingLibrary.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystematicStrategies
{
    internal class ComputationUtilities
    {
        public PricingLibrary.Computations.Pricer pricer;
        public DataUtilities dataUtilities;

        public ComputationUtilities(PricingLibrary.Computations.Pricer pricer, DataUtilities dataUtilities) 
        {
            this.pricer = pricer;   
            this.dataUtilities = dataUtilities;
        }

        public double GetFreeRate(DateTime date1, DateTime date2)
        {
            return PricingLibrary.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(date1, date2);
        }

        public double[] getSpots(DateTime date)
        {
            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(date);
            double[] spots = new double[marketDataCurrDate.Count];
            for (int i = 0; i < marketDataCurrDate.Count; i++)
            {
                spots[i] = marketDataCurrDate[i].Value;
            }
            return spots;
        }

        public double[] GetDeltas(DateTime date, DateTime maturity)
        {
            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(date);
            double[] spots = getSpots(date);
            return pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).Deltas;
        }

        public double[] GetDeltaStdDev(DateTime date, DateTime maturity)
        {
            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(date);
            double[] spots = getSpots(date);
            return pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).DeltaStdDev;
        }


        public double GetPrice(DateTime currDate, DateTime maturity)
        {
            return pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(currDate, maturity), getSpots(currDate)).Price;
        }

        public double GetPriceStdDev(DateTime date, DateTime maturity)
        {
            List<PricingLibrary.MarketDataFeed.ShareValue> marketDataCurrDate = dataUtilities.GetShareValuesForOneDate(date);
            double[] spots = getSpots(date);
            return pricer.Price(PricingLibrary.TimeHandler.MathDateConverter.ConvertToMathDistance(date, maturity), spots).PriceStdDev;
        }
    }


}
