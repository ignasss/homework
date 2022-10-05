using Application.Abstractions.Command;
using Models.Strategy;

namespace Application.Strategies.Commands.RegisterStrategy
{
    public sealed class RegisterStrategyCommand : ICommand
    {
        public string Ticker { get; }

        public Instruction Instruction { get; }

        public decimal PriceMovement { get; }

        public int Quantity { get; }

        public RegisterStrategyCommand(string ticker, Instruction instruction, decimal priceMovement, int quantity)
        {
            Ticker = ticker;
            Instruction = instruction;
            PriceMovement = priceMovement;
            Quantity = quantity;
        }
    }
}