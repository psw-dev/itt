using System;
using System.Reflection;
using System.Text.Json;
using PSW.Lib.Logs;
using PSW.RabbitMq;
using PSW.RabbitMq.RPC;
using PSW.RabbitMq.ServiceCommand;
using PSW.RabbitMq.Task;

namespace psw.itt.service.Helpers
{
    public class RPCHelper
    {
        public ServiceReply GetInformation(ServiceRequest serviceRequest, string remoteQueue)
        {
            var rpcClient = new RPCClient(remoteQueue);
            try
            {
                Log.Information("Class Name : RPCHelper | Method Name : {Method} | RabbitMq Request to {Queue} : {@svcRequest}", MethodBase.GetCurrentMethod().Name, remoteQueue, serviceRequest);
                var rabbitMQResponse = rpcClient.CallAsync(serviceRequest, remoteQueue).Result;
                var serviceReply = (ServiceReply)RabbitMqHelper.ByteArrayToObject(rabbitMQResponse);

                Log.Information($@"Class Name : RPCHelper | Method Name : {MethodBase.GetCurrentMethod().Name} | RabbitMq Response from {remoteQueue} : {serviceReply}");
                return serviceReply;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                rpcClient.Close();
            }
        }
        public T GetInformation<T>(ServiceRequest serviceRequest, string remoteQueue)
        {
            var rpcClient = new RPCClient(remoteQueue);
            try
            {
                Log.Information("Class Name : RPCHelper | Method Name : {Method} | RabbitMq Request to {Queue} : {@svcRequest}", MethodBase.GetCurrentMethod().Name, remoteQueue, serviceRequest);
                var rabbitMQResponse = rpcClient.CallAsync(serviceRequest, remoteQueue).Result;
                var responseBytes = (ServiceReply)RabbitMqHelper.ByteArrayToObject(rabbitMQResponse);
                if (responseBytes != null && responseBytes.data != null)
                {
                    var response = JsonSerializer.Deserialize<T>(responseBytes.data);
                    Log.Information("Class Name : RPCHelper | Method Name : {Method} | RabbitMq Request to {Queue} : {@response}", MethodBase.GetCurrentMethod().Name, remoteQueue, response);
                    return response;
                }

                Log.Information("Class Name : RPCHelper | Method Name : {Method} | RabbitMq Response from {Queue} : Null", MethodBase.GetCurrentMethod().Name, remoteQueue);
                return default(T);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                rpcClient.Close();
            }
        }
    }
}