using Application.Abstractions.Command;
using Models.Strategy;

namespace Application.Strategies.Commands.RegisterStrategy
{
    public class RegisterStrategyCommand : ICommand
    {
        public string Ticker { get; set; }
        public Instruction Instruction { get; set; }
        public decimal PriceMovement { get; set; }
        public int Quantity { get; set; }
    }
}