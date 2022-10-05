namespace Models.Strategy
{
    public sealed class StrategyDetails
    {
        public string Ticker { get; set; }
        public Instruction Instruction { get; set; }
        public decimal PriceMovement { get; set; }
        public int Quantity { get; set; }
        public decimal ExecutionPrice { get; set; }
    }
}