
using PSW.ITT.Service.Command;

namespace PSW.ITT.Service.Strategies
{
    public interface IStrategyFactory
    {
        Strategy CreateStrategy(CommandRequest request);
    }
}