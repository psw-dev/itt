using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using psw.itt.api.APICommand;
using psw.itt.data;
using psw.itt.service.IServices;
using PSW.Common.Crypto;
using psw.common.Extensions;
using Microsoft.AspNetCore.Http;
using psw.itt.service.Strategies;

namespace psw.itt.api.Controllers
{
    [Route("api/v1/[controller]")]
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
