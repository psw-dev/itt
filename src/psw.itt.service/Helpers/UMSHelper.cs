
using System.Text.Json;
using PSW.ITT.Common.Constants;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using PSW.RabbitMq.Task;
using PSW.Lib.Logs;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PSW.ITT.Service.Helpers
{
    public static class UMSHelper
    {
        /// <summary>
        /// GetUserInformation
        /// </summary>
        /// <param name="cmd">CommandRequest cmd</param>
        /// <param name="userRoleId">int? userRoleId</param>
        /// <returns>UserInformationResponseDTO</returns>
        public static UserInformationResponseDTO GetUserInformation(CommandRequest cmd, int? userRoleId)
        {
            var request = new UserInformationRequestDTO
            {
                UserRoleID = userRoleId
            };

            var rabbitMQResponse = RabbitMqCallHelper<UserInformationResponseDTO>.SyncCall(request, ServiceMethod.UMS_GET_DOCUMENT_ASSIGNEE, MessageQueues.UMSRPCQueue);

            if (rabbitMQResponse != null)
            {
                return rabbitMQResponse;
            }

            return null;
        }

    //     /// <summary>
    //     /// GetDocumentAssigneeInfo
    //     /// </summary>
    //     /// <param name="cmd">CommandRequest cmd</param>
    //     /// <param name="documents">List<DocumentRightsRequest> documents</param>
    //     /// <param name="documentClassificationCode">string documentClassificationCode</param>
    //     /// <param name="isActivityLog">bool isActivityLog = false</param>
    //     /// <returns>List<DocumentAssigneeInfoResponseDTO></returns>
    //     public static List<DocumentAssigneeInfoResponseDTO> GetDocumentAssigneeInfo(CommandRequest cmd, List<DocumentRightsRequest> documents, string documentClassificationCode, bool isActivityLog = false)
    //     {
    //         var request = new DocumentAssigneeInfoRequestDTO()
    //         {
    //             IsActivityLog = isActivityLog,
    //             AspNetUserId = cmd.AspNetUserId,
    //             DocumentClassificationCode = documentClassificationCode,
    //             DocumentAssigneeList = new List<DocumentAssignee>()

    //         };

    //         foreach (var document in documents)
    //         {

    //             request.DocumentAssigneeList.Add(new DocumentAssignee()
    //             {
    //                 UserRoleID = document.OfficerRoleID,
    //                 AgencyID = document.AgencyID,
    //                 DocumentTypeCode = document.DocumentTypeCode,
    //                 DocumentID = document.DocumentID,
    //                 IsLoggedEntry = document.IsLoggedEntry
    //             });
    //         }

    //         var rabbitMQResponse = RabbitMqCallHelper<List<DocumentAssigneeInfoResponseDTO>>.SyncCall(request, ServiceMethod.UMS_GET_DOCUMENT_ASSIGNEE_INFORMATION, MessageQueues.UMSRPCQueue);

    //         if (rabbitMQResponse != null)
    //         {
    //             return rabbitMQResponse;
    //         }

    //         return null;
    //     }

    }
}