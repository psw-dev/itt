
using psw.itt.service.Command;

namespace psw.itt.service.Strategies
{
    public interface IStrategyFactory
    {
        Strategy CreateStrategy(CommandRequest request);
    }
}