using System;
using System.ComponentModel;
using System.Text.Json;
using psw.itt.service.Command;
using PSW.ITT.Service.DTO;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using PSW.RabbitMq.Task;

namespace PSW.itt.Service.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// GetEnumDescription
        /// returns description attribute of enum
        /// </summary>
        /// <param name="enumValue">Enum</param>
        /// <returns>string</returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            try
            {
                var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
                var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}