using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PSW.ITT.Api.APICommand;
using PSW.ITT.Data;
using PSW.ITT.Service.IServices;
using PSW.Common.Crypto;
using psw.common.Extensions;
using Microsoft.AspNetCore.Http;
using PSW.ITT.Service.Strategies;
using psw.itt.service;

namespace PSW.ITT.Api.Controllers
{
    [Route("api/v1/itt/[controller]")]
    [ApiController]
    public class ITTController : BaseController
    {
        #region Constructors

        public ITTController(IITTService service, IITTOpenService openService, IUnitOfWork uow, IStrategyFactory strategyFactory, ICryptoAlgorithm cryptoAlgorithm, IHttpContextAccessor httpContextAccessor)
        : base(service, openService, uow, strategyFactory, cryptoAlgorithm, httpContextAccessor)
        {
        //     Service = service;
        //     Service.UnitOfWork = uow;
        //     Service.StrategyFactory = new StrategyFactory(uow);
        //     Service.CryptoAlgorithm = cryptoAlgorithm;

        // try{
        //      httpContextAccessor.HttpContext.Request.Headers.TryGetValue("LoggedInUserRoleID", out var userRoleId);
        //     service.LoggedInUserRoleId = cryptoAlgorithm.Decrypt(userRoleId).ToIntOrDefault();
        // }
        // catch{

        // }
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
