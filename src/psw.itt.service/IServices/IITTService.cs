using System.Collections.Generic;
using System.Security.Claims;

namespace PSW.ITT.Service.IServices
{
    public interface IITTService : IService
    {
        // CommandReply invokeMethod(CommandRequest request);
         IEnumerable<Claim> UserClaims { get; set; }
    }

}