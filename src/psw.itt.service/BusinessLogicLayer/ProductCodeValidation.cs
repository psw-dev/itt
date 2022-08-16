using System;
using System.Text.RegularExpressions;
using PSW.ITT.Service.Command;
using PSW.Lib.Logs;


namespace PSW.ITT.Service.BusinessLogicLayer
{
    public class ProductCodeValidation
    {
        private string hSCode;
        private string productCode;
        private DateTime effectiveFromDt;
        private DateTime effectiveThruDt;
        private CommandRequest command;
        public ProductCodeValidation(string hSCode, string productCode, DateTime effectiveFromDt, DateTime effectiveThruDt, CommandRequest command)
        {
            this.hSCode = hSCode;
            this.productCode = hSCode;
            this.effectiveFromDt = effectiveFromDt;
            this.effectiveThruDt = effectiveThruDt;
            this.command = command;
        }

        public void validate()
        {
            if (String.IsNullOrEmpty(hSCode) || String.IsNullOrEmpty(productCode))
            {
                Log.Error($"|ProductCodeValidation| Empty HSCode or Product Code");
                throw new System.Exception($"Empty HSCode or Product Code");
            }

            // Product code effective from date can not be set as a previous date. More preciously, the effective date should always be current or future date.
            if (effectiveFromDt < DateTime.Now)
            {
                Log.Error($"|ProductCodeValidation| Product code effective from date can not be set as a previous date");
                throw new System.Exception($"the effective date {effectiveFromDt} should always be current or future date");
            }

            // Product code end date can not be set as a previous date. It should always be today's or future date.
            if (effectiveThruDt < DateTime.Now)
            {
                Log.Error($"|ProductCodeValidation| Product code end date can not be set as a previous date");
                throw new System.Exception($"the effective thru {effectiveThruDt} should always be current or future date");
            }
            // HSCode lenght should be 8 digits (numeric value)
            string pattern = @"([0-9]{4})\.([0-9]{4})";
            string input = hSCode;
            if (input.Length != 9)
            {
                Log.Error($"|ProductCodeValidation| Invalid Hscode");
                throw new System.Exception($"Invalid Hscode {hSCode}");
            }
            Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                Log.Error($"|ProductCodeValidation| Invalid Hscode");
                throw new System.Exception($"Invalid Hscode {hSCode}");
            }

            // Product Code should be 4 digits(numeric value)

            if (productCode.Length != 4)
            {
                Log.Error($"|ProductCodeValidation| Invalid Product Code");
                throw new System.Exception($"Invalid Product Code {productCode}");
            }
            match = Regex.Match(input, @"[0-9]{4})", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                Log.Error($"|ProductCodeValidation| Invalid Product Code");
                throw new System.Exception($"Invalid Product Code {productCode}");
            }

            // There should be no active same HsCode + Product Code combination having overlapping effective date and end date. 

            var overLappingProductCode = command.UnitOfWork.ProductCodeEntityRepository.GetOverlappingProductCode(hSCode, productCode, effectiveFromDt, effectiveThruDt);
            if (overLappingProductCode.Count > 0)
            {
                Log.Error($"|ProductCodeValidation| Product Code is overlapping with existing product code");
                throw new System.Exception($"Invalid! Product Code is overlapping with existing product code");
            }


        }

    }
}