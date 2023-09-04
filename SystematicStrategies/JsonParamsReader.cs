using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.DataClasses;

namespace SystematicStrategies
{
    public class JsonParamsReader
    {
        public static BasketTestParameters GetParam(string jsonPath)
        {
            BasketTestParameters testParameters = null;
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string json = r.ReadToEnd();

                var model = System.Text.Json.JsonSerializer.Deserialize<BasketTestParametersModel>(json);

                testParameters = new BasketTestParameters
                {
                    PricingParams = new BasketPricingParameters
                    {
                        Volatilities = model.pricingParams.volatilities,
                        Correlations = model.pricingParams.correlations
                    },
                    BasketOption = new Basket
                    {
                        Strike = model.basketOption.strike,
                        Maturity = model.basketOption.maturity,
                        UnderlyingShareIds = model.basketOption.underlyingShareIds,
                        Weights = model.basketOption.weights
                    },
                    RebalancingOracleDescription = new PricingLibrary.RebalancingOracleDescriptions.RegularOracleDescription
                    {
                        Period = model.rebalancingOracleDescription.period
                    }
                };
            }
            return testParameters;
        }
    }
}
