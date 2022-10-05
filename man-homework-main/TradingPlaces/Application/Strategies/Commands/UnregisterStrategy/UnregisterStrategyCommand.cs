using Application.Abstractions.Command;

namespace Application.Strategies.Commands.UnregisterStrategy
{
    public sealed class UnregisterStrategyCommand : ICommand
    {
        public string Id { get; }

        public UnregisterStrategyCommand(string id)
        {
            Id = id;
        }
    }
}