namespace Models.Strategy
{
    public class StrategyDetails
    {
        public string Ticker { get; set; }
        public Instruction Instruction { get; set; }
        public decimal PriceMovement { get; set; }
        public int Quantity { get; set; }
    }
}