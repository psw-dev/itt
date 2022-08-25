using System.Collections.Generic;
using System.Security.Claims;

namespace psw.itt.service
{
    public interface IITTService : IService
    {
        // CommandReply invokeMethod(CommandRequest request);
         IEnumerable<Claim> UserClaims { get; set; }
    }

}