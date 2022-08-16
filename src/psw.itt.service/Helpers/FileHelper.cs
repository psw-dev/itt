using System.Text.Json;
using PSW.ITT.Common.Constants;
using PSW.ITT.Service.Command;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using PSW.ITT.Service.DTO;
using System.Collections.Generic;
using System.Reflection;
using PSW.Lib.Logs;

namespace  PSW.ITT.Service.Helpers
{
    public static class FileHelper
    { 
        public static GetFilesDetailResponseDTO GetFilesDetails(CommandRequest cmd, List<string> files)
        {
            var requestDto = new GetFilesDetailRequestDTO
            {
                Files = files
            };

            var svcRequest = new ServiceRequest
            {
                methodId = Constant.GetFileDetails,
                data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(requestDto)).RootElement.GetRawText()
            };

            Log.Information("[{0}.{1}] RabbitMq Request to {2}: {@svcRequest}", "FileHelper", MethodBase.GetCurrentMethod().Name, MessageQueues.FSSRPCQueue, svcRequest);

            var rabbitMQResponse = new RPCHelper().GetInformation(svcRequest, MessageQueues.FSSRPCQueue);

            Log.Information("[{0}.{1}] RabbitMq Response from {2}: {@rabbitMQResponse}", "FileHelper", MethodBase.GetCurrentMethod().Name, MessageQueues.FSSRPCQueue, rabbitMQResponse);

            if (rabbitMQResponse?.data != null)
            {
                var response = JsonSerializer.Deserialize<GetFilesDetailResponseDTO>(rabbitMQResponse.data);
                return response;
            }

            return null;
        }
    }
}