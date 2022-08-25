using PSW.ITT.Data;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.Strategies;
using PSW.Common.Crypto;

namespace PSW.ITT.Service.IServices
{
    public interface IService
    {
        IUnitOfWork UnitOfWork { get; set; }
        IStrategyFactory StrategyFactory { get; set; }
        CommandReply invokeMethod(CommandRequest request);
        ICryptoAlgorithm CryptoAlgorithm { get; set; }
        int LoggedInUserRoleId { get; set; }
    }

}