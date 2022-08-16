using System;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSW.ITT.Api.APICommand;
using PSW.ITT.Common.Pagination;
using PSW.ITT.Data;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.IServices;
using PSW.ITT.Service.Strategies;
using PSW.Common.Crypto;
using psw.common.Extensions;

namespace PSW.ITT.Api.Controllers
{
    public class BaseController : Controller
    {
        #region Properties

        public IService Service { get; set; }

        #endregion

        #region Controller

        public BaseController(IService service, IUnitOfWork uow, IStrategyFactory strategyFactory, ICryptoAlgorithm cryptoAlgorithm, IHttpContextAccessor httpContextAccessor)
        {
            // Dependency Injection of services
            Service = service;
            Service.UnitOfWork = uow;
            Service.StrategyFactory = strategyFactory;
            Service.CryptoAlgorithm = cryptoAlgorithm;
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("LoggedInUserRoleID", out var userRoleId);
            service.LoggedInUserRoleId = cryptoAlgorithm.Decrypt(userRoleId).ToIntOrDefault();
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

        #endregion
    }
}