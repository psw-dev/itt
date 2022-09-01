using System;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSW.ITT.Api.APICommand;
using PSW.ITT.Common.Pagination;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.IServices;
using PSW.ITT.Service.Strategies;
using PSW.Common.Crypto;
using psw.common.Extensions;
using PSW.Lib.Logs;
using System.Threading.Tasks;
using PSW.ITT.Data;

namespace PSW.ITT.Api.Controllers
{
    public class BaseController : Controller
    {
        #region Properties

        public IService Service { get; set; }
        public IService OpenService { get; set; }

        #endregion

        #region Controller

        public BaseController(IService service, IITTOpenService openService, IUnitOfWork uow, IStrategyFactory strategyFactory, ICryptoAlgorithm cryptoAlgorithm, IHttpContextAccessor httpContextAccessor)
        {
            // Dependency Injection of services
            Service = service;
            Service.UnitOfWork = uow;
            Service.StrategyFactory = new StrategyFactory(uow);
            Service.CryptoAlgorithm = cryptoAlgorithm;

            OpenService = openService;
            OpenService.UnitOfWork = uow;
            OpenService.StrategyFactory = new OpenStrategyFactory(uow);
            OpenService.CryptoAlgorithm = cryptoAlgorithm;
            try
            {
                httpContextAccessor.HttpContext.Request.Headers.TryGetValue("LoggedInUserRoleID", out var userRoleId);
                service.LoggedInUserRoleId = cryptoAlgorithm.Decrypt(userRoleId).ToIntOrDefault();
            }
            catch { }
        }

        #endregion

        #region End Points 

        // TODO: Authentication
        // Assuming Request is Authenticated.
        [HttpPost("secure")]
        [Authorize("authorizedUserPolicy")]
        public virtual ActionResult<APIResponse> SecureRequest(APIRequest apiRequest)
        {
            //TODO: Client Id Authentication here
            var apiResponse = new APIResponse
            {
                methodId = apiRequest.methodId,
                message = new ResponseMessage
                {
                    code = "404",
                    description = "Action not found."
                },
                //TODO: Add error object if required
                error = new ErrorModel(),
                //TODO: Add pagination if required
                pagination = new ServerPaginationModel()
            };

            try
            {
                //TODO: Resource Authorization (Middleware)
                //TODO: Pass User Details and Method ID to Middleware for Action/Method/Resource Authorization

                // Assuming Request is Authenticated
                // Crate JsonElement for service
                // Calling Service 
                CommandReply commandReply = Service.invokeMethod(
                    new CommandRequest
                    {
                        methodId = apiRequest.methodId,
                        data = apiRequest.data,
                        CurrentUser = HttpContext.User,
                        UserClaims = HttpContext.User.Claims,
                        LoggedInUserRoleID = Service.LoggedInUserRoleId,
                        pagination = apiRequest.pagination
                    }
                );

                // Preparing Response 
                apiResponse = ApiResponseByCommand(commandReply, apiResponse);
            }
            catch (Exception ex)
            {
                // Prepare Appropriate Response
                apiResponse.message.code = "404";
                apiResponse.message.description = "Error : " + ex.Message;
            }

            // Send Response 
            return apiResponse;
        }


        protected string Sign(JsonElement data)
        {
            // TODO Call Sign API to Get Signature  
            using HashAlgorithm algorithm = SHA256.Create();
            return Convert.ToBase64String(algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data.ToString() ?? string.Empty)));
        }

        protected APIResponse ApiResponseByCommand(CommandReply commandReply, APIResponse apiResponse)
        {
            try
            {
                apiResponse.data = commandReply.data; // TODO: Safe Check on Data
                string signature = Sign(commandReply.data); // TODO: Safe Check Data
                apiResponse.signature = signature; // Sign Data 

                apiResponse.message.code = string.IsNullOrEmpty(commandReply.code) ? "400" : commandReply.code;
                apiResponse.message.description = string.IsNullOrEmpty(commandReply.message) ? "Bad Request" : commandReply.message;

                if (apiResponse.error != null)
                {
                    apiResponse.error.exception = string.IsNullOrEmpty(commandReply.exception) ? "" : commandReply.exception;
                    apiResponse.error.shortDescription = string.IsNullOrEmpty(commandReply.shortDescription) ? "" : commandReply.shortDescription;
                    apiResponse.error.fullDescription = string.IsNullOrEmpty(commandReply.fullDescription) ? "" : commandReply.fullDescription;
                }
                if (apiResponse.pagination != null)
                {
                    apiResponse.pagination = commandReply.pagination;
                }
            }
            catch (Exception ex)
            {
                apiResponse.message.code = "400";
                apiResponse.message.description = "Cannot Prepare Response";
                if (apiResponse.error != null) apiResponse.error.exception = "Exception : " + ex;
            }

            return apiResponse;

        }

        [HttpPost("open")]
        public virtual ActionResult<object> OpenRequest(APIRequest apiRequest)
        {
            var apiResponse = new APIResponse
            {
                methodId = apiRequest.methodId,
                message = new ResponseMessage()
            };

            try
            {
                //TODO: Resource Authorization (Middleware)
                //TODO: Pass User Details and Method ID to Middleware for Action/Method/Resource Authorization

                // Assuming Request is Authenticated
                // Calling Service 
                CommandReply commandReply = OpenService.invokeMethod(
                    new CommandRequest
                    {
                        methodId = apiRequest.methodId,
                        data = apiRequest.data
                    }
                );

                // Preparing Response 
                apiResponse = ApiResponseByCommand(commandReply, apiResponse);
            }
            catch (Exception ex)
            {
                // Prepare Appropriate Response
                apiResponse.message.code = "404";
                apiResponse.message.description = "Error : " + ex.Message;
            }

            // Send Response 
            return apiResponse;
        }

        [HttpGet("query/{methodId}")]
        public virtual ActionResult<object> OpenRequest(string methodId, string username, int duration)
        {
            //TODO: Client Id Authentication here
            var apiResponse = new APIResponse
            {
                methodId = methodId,
                message = new ResponseMessage
                {
                    code = "404",
                    description = "Action not found."
                },
                //TODO: Add error object if required
                error = null,
                //TODO: Add pagination if required
                pagination = null
            };

            try
            {
                //TODO: Resource Authorization (Middleware)
                //TODO: Pass User Details and Method ID to Middleware for Action/Method/Resource Authorization

                // Assuming Request is Authenticated
                // Crate JsonElement for service
                string json = "{" + $@"""username"":""{username}"",""duration"":{duration}" + "}";
                JsonElement element = JsonDocument.Parse(json).RootElement;

                // Calling Service 
                CommandReply commandReply = OpenService.invokeMethod(
                    new CommandRequest
                    {
                        methodId = methodId,
                        data = element
                    }
                );

                // Preparing Response
                apiResponse.data = commandReply.data;
                apiResponse.message.code = commandReply.code;
                apiResponse.message.description = commandReply.message;

                // Sign Data
                string signature = Sign(commandReply.data);
                apiResponse.signature = signature;
            }
            catch (Exception ex)
            {
                // Prepare Appropriate Response 
                apiResponse.message.code = "404";
                apiResponse.message.description = "Error : " + ex.Message;
            }

            // Send Response 
            return apiResponse;
        }

        [HttpGet("version")]
        public virtual ActionResult<object> GetVersion()
        {
            return "20210208_081653";
        }

        [HttpPost("fileRegistration")]
        //public virtual ActionResult<APIResponse> FileRegistration([FromForm] APIRequestFileRegistration apiRequest)
        public virtual ActionResult<APIResponse> FileRegistration([FromForm] APIRequestFile apiRequest)
        {
            string path = "";
            var apiResponse = new APIResponse
            {
                methodId = apiRequest.methodId,
                message = new ResponseMessage
                {
                    code = "404",
                    description = "Action not found."
                },
                //TODO: Add error object if required
                error = new ErrorModel(),
                //TODO: Add pagination if required
                pagination = new ServerPaginationModel()
            };
            try
            {
                PSW.ITT.Api.APICommand.Data data = new PSW.ITT.Api.APICommand.Data
                {
                    filepath = apiRequest.FilePath,
                    fileId = apiRequest.fileId,
                    fileName = apiRequest.fileName,
                    roleCode = apiRequest.roleCode
                };
                //TODO: Resourse Authorization (Middleware)
                //TODO: Pass User Detials and Method ID to Middleware for Action/Method/Resourse Authorization
                // Assuming Request is Authenticated 
                // bool authenticated = true;
                // if (authenticated)
                // {
                // Calling Service 

                CommandReply commandReply = Service.invokeMethod(
                new CommandRequest()
                {
                    methodId = apiRequest.methodId,
                    data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(data)).RootElement,
                    CurrentUser = HttpContext.User,
                    UserClaims = HttpContext.User.Claims,
                    pagination = apiRequest.pagination,
                    file = apiRequest.file,
                    roleCode = apiRequest.roleCode
                }
                );
                apiResponse = ApiResponseByCommand(commandReply, apiResponse);

                Log.Information($"|FileRegistration| Path {path}.");
                if (commandReply.code == "400" || commandReply.code == "500")
                {
                    // return BadRequest(commandReply.message);
                }
                path = commandReply.message;
                Log.Information($"|FileRegistration| Path {path}.");




            }
            catch (Exception ex)
            {
                Log.Error($"BaseController {ex.ToString()}");
                return BadRequest(ex.Message);
            }
            return apiResponse;

        }
        #endregion
    }
}