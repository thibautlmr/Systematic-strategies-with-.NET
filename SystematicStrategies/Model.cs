using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematicStrategies
{
    public class BasketTestParametersModel
    {
        public BasketPricingParametersModel pricingParams { get; set; }
        public BasketModel basketOption { get; set; }
        public RebalancingOracleDescriptionModel rebalancingOracleDescription { get; set; }
    }

    public class BasketPricingParametersModel
    {
        public double[] volatilities { get; set; }
        public double[][] correlations { get; set; }
    }

    public class BasketModel
    {
        public double strike { get; set; }
        public DateTime maturity { get; set; }
        public string[] underlyingShareIds { get; set; }
        public double[] weights { get; set; }
    }

    public class RebalancingOracleDescriptionModel
    {
        public int period { get; set; }
        public string type { get; set; }
    }
}
