using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
namespace BacktestLibrary
{
    public class Portfolio
    {
        public Dictionary<string, double> Composition;
        public double FreeRateQuantity;
        public double FreeRate;
        public Portfolio(Dictionary<string, double> composition, double freeRateQuantity, double freeRate)
        {
            Composition = composition;
            FreeRateQuantity = freeRateQuantity;
            FreeRate = freeRate;
        }
    }
}
