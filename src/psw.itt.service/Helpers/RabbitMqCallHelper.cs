
using System.Text.Json;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using PSW.ITT.Service.DTO;
using PSW.Lib.Logs;
using System;
using System.Reflection;
using System.Net;

namespace  PSW.ITT.Service.Helpers
{
    /// <summary>
    ///     SDHelper
    /// </summary>
    public static class RabbitMqCallHelper<T> where T : class
    {
        /// <summary>
        /// SyncCall
        /// </summary>
        /// <param name="request">object request</param>
        /// <param name="methodId">string methodId</param>
        /// <param name="queueName">string queueName</param>
        /// <returns></returns>
        public static T SyncCall(object request, string methodId, string queueName)
        {
            var rpcHelper = new RPCHelper();
            try
            {
                var svcRequest = new ServiceRequest()
                {
                    methodId = methodId,
                    data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(request)).RootElement.GetRawText()
                };

                Log.Information("[{0}.{1}] RabbitMq Request to {2}: {svcRequest}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, queueName, svcRequest);

                var rabbitMQResponse = rpcHelper.GetInformation(svcRequest, queueName);

                Log.Information("[{0}.{1}] RabbitMq Response from {2}: {rabbitMQResponse}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, queueName, rabbitMQResponse);

                if (rabbitMQResponse != null && rabbitMQResponse.code == ((int)HttpStatusCode.OK).ToString())
                {
                    var response = JsonSerializer.Deserialize<T>(rabbitMQResponse.data);
                    return response;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public static ServiceReply SyncCallServiceReply(object request, string methodId, string queueName)
        {
            var rpcHelper = new RPCHelper();
            try
            {
                var svcRequest = new ServiceRequest()
                {
                    methodId = methodId,
                    data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(request)).RootElement.GetRawText()
                };

                Log.Information("[{0}.{1}] RabbitMq Request to {2}: {svcRequest}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, queueName, svcRequest);

                var rabbitMQResponse = rpcHelper.GetInformation(svcRequest, queueName);

                Log.Information("[{0}.{1}] RabbitMq Response from {2}: {rabbitMQResponse}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, queueName, rabbitMQResponse);

                if (rabbitMQResponse != null && rabbitMQResponse.code == ((int)HttpStatusCode.OK).ToString())
                {
                    return rabbitMQResponse;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}", "RabbitMqCallHelper", MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }
    }
}