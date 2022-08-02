using psw.itt.data;
using psw.itt.service.Command;
using psw.itt.service.Strategies;
using PSW.Common.Crypto;

namespace psw.itt.service.IServices
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