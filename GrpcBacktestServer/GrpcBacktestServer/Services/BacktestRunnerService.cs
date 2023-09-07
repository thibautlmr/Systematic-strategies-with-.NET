using Grpc.Core;
using GrpcBacktestServer;
using BacktestLibrary;
using Google.Protobuf.WellKnownTypes;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;

namespace GrpcBacktestServer.Services

{
    public class BacktestRunnerService : BacktestRunner.BacktestRunnerBase
    {
        public override Task<BacktestOutput> RunBacktest(BacktestRequest request, ServerCallContext context)
        {
            var marketDataProto = request.Data;
            var testParametersProto = request.TstParams;

            var marketData = new List<ShareValue>();
            foreach (var shareDataProto in marketDataProto.DataValues)
            {
                DateTime dateTime = shareDataProto.Date.ToDateTime();

                var shareValue = new ShareValue
                {
                    DateOfPrice = dateTime,
                    Id = shareDataProto.Id,
                    Value = shareDataProto.Value
                };

                marketData.Add(shareValue);
            }


            var basketOption = new Basket
            {
                Maturity = testParametersProto.BasketParams.Maturity.ToDateTime(),
                Strike = testParametersProto.BasketParams.Strike,
                UnderlyingShareIds = testParametersProto.BasketParams.ShareIds.ToArray(),
                Weights = testParametersProto.BasketParams.Weights.ToArray()
            };

            var basketPricingParameters = new BasketPricingParameters
            {
                Correlations = testParametersProto.PriceParams.Corrs.Select(corr => corr.Value.ToArray()).ToArray(),
                Volatilities = testParametersProto.PriceParams.Vols.ToArray()
            };

            var testParameters = new BasketTestParameters
            {
                BasketOption = basketOption,
                PricingParams = basketPricingParameters
            };

            if (testParametersProto.RebParams.RebTypeCase == RebalancingParams.RebTypeOneofCase.Regular)
            {
                testParameters.RebalancingOracleDescription = new RegularOracleDescription();
                ((RegularOracleDescription)testParameters.RebalancingOracleDescription).Period = testParametersProto.RebParams.Regular.Period;
            } else
            {
                testParameters.RebalancingOracleDescription = new WeeklyOracleDescription();
                ((WeeklyOracleDescription)testParameters.RebalancingOracleDescription).RebalancingDay = (DayOfWeek)testParametersProto.RebParams.Weekly.Day;
            }

            Hedging hedging = new();
            string outputFilePath = "C:\\Users\\Erwan Izenic\\OneDrive\\Documents\\COURS_3A\\Systematic-strategies-with-.NET\\SystematicStrategies\\erwan.json";
            hedging.Run(marketData, testParameters, outputFilePath);

            BacktestOutput output = new BacktestOutput();
            foreach (OutputData outputData in hedging.OutputData)
            {
                
                BacktestInfo backtestInfo = new BacktestInfo
                {
                    Date = Timestamp.FromDateTime(outputData.Date),
                    PortfolioValue = outputData.Value,
                    Delta = { outputData.Deltas }, 
                    DeltaStddev = { outputData.DeltasStdDev }, 
                    Price = outputData.Price,
                    PriceStddev = outputData.PriceStdDev
                };
                output.BacktestInfo.Add(backtestInfo);
            }


            return Task.FromResult(output);
        }
    }
}