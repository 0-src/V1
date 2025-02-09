using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace V1.Scripts.Classes
{
    public class AccountData
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("equity")]
        public string EquityString { get; set; } // Keep as string for raw JSON

        [JsonIgnore]
        public decimal Equity => DataConverter.ConvertToDecimal(EquityString); // Convert to decimal

        [JsonProperty("performanceData")]
        public PerformanceData PerformanceData { get; set; }
    }

    public class PerformanceData
    {
        [JsonProperty("All Trades")]
        public AllTrades AllTrades { get; set; }

        [JsonProperty("Profit trades")]
        public ProfitTrades ProfitTrades { get; set; }

        [JsonProperty("Losing trades")]
        public LosingTrades LosingTrades { get; set; }

        [JsonProperty("Trades")]
        public List<Trade> Trades { get; set; }
    }

    public class AllTrades
    {
        [JsonProperty("Gross P/L")]
        public string GrossPLString { get; set; }

        [JsonIgnore]
        public decimal GrossPL => DataConverter.ConvertToDecimal(GrossPLString);

        [JsonProperty("# of Trades")]
        public int NumberOfTrades { get; set; }

        [JsonProperty("# of Contracts")]
        public int NumberOfContracts { get; set; }

        [JsonProperty("Avg. Trade Time")]
        public string AverageTradeTime { get; set; }

        [JsonProperty("Longest Trade Time")]
        public string LongestTradeTime { get; set; }

        [JsonProperty("% Profitable Trades")]
        public string ProfitableTrades { get; set; }

        [JsonProperty("Expectancy")]
        public string ExpectancyString { get; set; }

        [JsonIgnore]
        public decimal Expectancy => DataConverter.ConvertToDecimal(ExpectancyString);

        [JsonProperty("Trade Fees & Comm.")]
        public string TradeFeesAndCommissionString { get; set; }

        [JsonIgnore]
        public decimal TradeFeesAndCommission => DataConverter.ConvertToDecimal(TradeFeesAndCommissionString);

        [JsonProperty("Total P/L")]
        public string TotalPLString { get; set; }

        [JsonIgnore]
        public decimal TotalPL => DataConverter.ConvertToDecimal(TotalPLString);
    }

    public class ProfitTrades
    {
        [JsonProperty("Total Profit")]
        public string TotalProfitString { get; set; }

        [JsonIgnore]
        public decimal TotalProfit => DataConverter.ConvertToDecimal(TotalProfitString);

        [JsonProperty("# of Winning Trades")]
        public int NumberOfWinningTrades { get; set; }

        [JsonProperty("# of Winning Contracts")]
        public int NumberOfWinningContracts { get; set; }

        [JsonProperty("Largest Winning Trade")]
        public string LargestWinningTradeString { get; set; }

        [JsonIgnore]
        public decimal LargestWinningTrade => DataConverter.ConvertToDecimal(LargestWinningTradeString);

        [JsonProperty("Avg. Winning Trade")]
        public string AverageWinningTradeString { get; set; }

        [JsonIgnore]
        public decimal AverageWinningTrade => DataConverter.ConvertToDecimal(AverageWinningTradeString);

        [JsonProperty("Std. Dev. Winning Trade")]
        public string StdDevWinningTrade { get; set; }

        [JsonProperty("Avg. Winning Trade Time")]
        public string AverageWinningTradeTime { get; set; }

        [JsonProperty("Longest Winning Trade Time")]
        public string LongestWinningTradeTime { get; set; }

        [JsonProperty("Max Run-up")]
        public string MaxRunUpString { get; set; }

        [JsonIgnore]
        public decimal MaxRunUp => DataConverter.ConvertToDecimal(MaxRunUpString);
    }

    public class LosingTrades
    {
        [JsonProperty("Total Loss")]
        public string TotalLossString { get; set; }

        [JsonIgnore]
        public decimal TotalLoss => DataConverter.ConvertToDecimal(TotalLossString);

        [JsonProperty("# of Losing Trades")]
        public int NumberOfLosingTrades { get; set; }

        [JsonProperty("# of Losing Contracts")]
        public int NumberOfLosingContracts { get; set; }

        [JsonProperty("Largest Losing Trade")]
        public string LargestLosingTradeString { get; set; }

        [JsonIgnore]
        public decimal LargestLosingTrade => DataConverter.ConvertToDecimal(LargestLosingTradeString);

        [JsonProperty("Avg. Losing Trade")]
        public string AverageLosingTradeString { get; set; }

        [JsonIgnore]
        public decimal AverageLosingTrade => DataConverter.ConvertToDecimal(AverageLosingTradeString);

        [JsonProperty("Std. Dev. Losing Trade")]
        public string StdDevLosingTrade { get; set; }

        [JsonProperty("Avg. Losing Trade Time")]
        public string AverageLosingTradeTime { get; set; }

        [JsonProperty("Longest Losing Trade Time")]
        public string LongestLosingTradeTime { get; set; }

        [JsonProperty("Max Drawdown")]
        public string MaxDrawdownString { get; set; }

        [JsonIgnore]
        public decimal MaxDrawdown => DataConverter.ConvertToDecimal(MaxDrawdownString);
    }

    public class Trade
    {
        [JsonProperty("Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("Qty")]
        public int Qty { get; set; }

        [JsonProperty("Buy Price")]
        public string BuyPriceString { get; set; }

        [JsonIgnore]
        public decimal BuyPrice => DataConverter.ConvertToDecimal(BuyPriceString);

        [JsonProperty("Buy Time")]
        public string BuyTime { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("Sell Time")]
        public string SellTime { get; set; }

        [JsonProperty("Sell Price")]
        public string SellPriceString { get; set; }

        [JsonIgnore]
        public decimal SellPrice => DataConverter.ConvertToDecimal(SellPriceString);

        [JsonProperty("P&L")]
        public string PLString { get; set; }

        [JsonIgnore]
        public decimal PL => DataConverter.ConvertToDecimal(PLString);
    }

    public static class DataConverter
    {
        public static decimal ConvertToDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0m; // Default to 0 if empty

            bool isNegative = input.StartsWith("(") && input.EndsWith(")");
            input = input.Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",", "").Trim(); // Remove $, (), , and spaces

            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return isNegative ? -result : result;

            return 0m; // Return 0 if conversion fails
        }
    }
}
