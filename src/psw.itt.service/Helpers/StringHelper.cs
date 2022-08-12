
using System.Text.Json;
using PSW.itt.Common.Constants;
using psw.itt.service.Command;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using PSW.RabbitMq.Task;
using PSW.ITT.Service.DTO;
using PSW.Lib.Logs;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace psw.oga.service.Helpers
{
    public static class StringHelper
    {
        public static string IsNull(string str)
        {
            try
            {
                return string.IsNullOrWhiteSpace(str) ? "N/A" : Convert.ToString(str);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string CamelCase(string s)
        {
            var x = s.Replace("_", " ");
            if (x.Length == 0) return "null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
            return char.ToUpper(x[0]) + x.Substring(1);
        }
    }


}