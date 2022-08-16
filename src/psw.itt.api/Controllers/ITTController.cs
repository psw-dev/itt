using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PSW.ITT.Api.APICommand;
using PSW.ITT.Data;
using PSW.ITT.Service.IServices;
using PSW.Common.Crypto;
using psw.common.Extensions;
using Microsoft.AspNetCore.Http;
using PSW.ITT.Service.Strategies;

namespace PSW.ITT.Api.Controllers
{
    [Route("api/v1/itt/[controller]")]
    [ApiController]
    public class ITTController : BaseController
    {
        #region Constructors

        public ITTController(IITTService service, IUnitOfWork uow, IStrategyFactory strategyFactory, ICryptoAlgorithm cryptoAlgorithm, IHttpContextAccessor httpContextAccessor)
        : base(service, uow, strategyFactory, cryptoAlgorithm, httpContextAccessor)
        {
        }

        #endregion

        #region End Points 

        [HttpPost("secure")]
        [Authorize("authorizedUserPolicy")]
        public override ActionResult<APIResponse> SecureRequest(APIRequest apiRequest)
        {
            return base.SecureRequest(apiRequest);
        }


        [HttpGet("env")]
        public ActionResult<object> GetEnv()
        {
            return System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        #endregion
    }
}
