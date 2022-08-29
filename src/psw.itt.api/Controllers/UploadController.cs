using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using psw.itt.service;
using PSW.Common.Crypto;
using psw.common.Extensions;
using Microsoft.AspNetCore.Http;
using PSW.ITT.Api.APICommand;
using PSW.ITT.Service.IServices;
using PSW.ITT.Data;
using PSW.ITT.Service.Strategies;

namespace PSW.ITT.Api.Controllers
{
    [Route("api/v1/itt/[controller]")]
    [ApiController]
    public class UploadController : BaseController
    {
        #region Constructors

        public UploadController(IITTService secureService, IITTOpenService openService, IUnitOfWork uow, IStrategyFactory strategyFactory, ICryptoAlgorithm cryptoAlgorithm, IHttpContextAccessor httpContextAccessor) : 
        base(secureService, openService, uow, strategyFactory, cryptoAlgorithm, httpContextAccessor)
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

        [HttpPost("open")]
        public override ActionResult<object> OpenRequest(APIRequest apiRequest)
        {
            return base.OpenRequest(apiRequest);
        }

        [HttpGet("query/{methodId}")]
        public override ActionResult<object> OpenRequest(string methodId, string username, int duration)
        {
            return base.OpenRequest(methodId, username, duration);
        }

        [HttpGet("version")]
        public override ActionResult<object> GetVersion()
        {
            return "20210219_144630";
        }

        [HttpGet("env")]
        public ActionResult<object> GetEnv()
        {
            return System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        
        [HttpPost("fileUpload")]
        [Authorize("authorizedUserPolicy")]
        public override ActionResult<APIResponse> FileRegistration([FromForm] APIRequestFile apiRequest)
        {
            return base.FileRegistration(apiRequest);
        }

        
        #endregion
    }
}