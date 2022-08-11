using System;
using System.Text.RegularExpressions;
using psw.itt.service.Command;
using PSW.Lib.Logs;


namespace BLL
{
    public class ProductCodeValidation
    {
        /*
                Validations to be added in the activity diagram:

              
                6. Once a product code is end dated (Deactivated), it cannot be re-activated. The same product code with new effective and end date range should be added in the system.
                8. 
                11. Product code validity should be within the validity of referenced HSCode and it should not go beyond the validity of the referenced HScode.
                12. In excel upload/screen, user will be allowed either to enter the End Date or select "infinate Date" checkbox at a time.
        */
        string hSCode;
        string productCode;
        DateTime effectiveFromDt;
        DateTime effectiveThruDt;
        CommandRequest command;
        public ProductCodeValidation(string hSCode, string productCode, DateTime effectiveFromDt, DateTime effectiveThruDt, CommandRequest command)
        {
            this.hSCode = hSCode;
            this.productCode = hSCode;
            this.effectiveFromDt = effectiveFromDt;
            this.effectiveThruDt = effectiveThruDt;
            this.command = command;
            if (String.IsNullOrEmpty(hSCode) || String.IsNullOrEmpty(productCode))
            {
                Log.Error($"|ProductCodeValidation| Empty HSCode or Product Code");
                throw new System.Exception($"Empty HSCode or Product Code");
            }

            // Product code effective from date can not be set as a previous date. More preciously, the effective date should always be current or future date.
            if (effectiveFromDt < DateTime.Now)
            {
                Log.Error($"|ProductCodeValidation| Product code effective from date can not be set as a previous date");
                throw new System.Exception($"the effective date should always be current or future date");
            }

            // Product code end date can not be set as a previous date. It should always be today's or future date.
            if (effectiveThruDt < DateTime.Now)
            {
                Log.Error($"|ProductCodeValidation| Product code end date can not be set as a previous date");
                throw new System.Exception($"the effective thru should always be current or future date");
            }
        }

        public void validate()
        {
            // HSCode lenght should be 8 digits (numeric value)
            string pattern = @"([0-9]{4})\.([0-9]{4})";
            string input = hSCode;
            if (input.Length != 9)
            {
                Log.Error($"|ProductCodeValidation| Invalid Hscode");
                throw new System.Exception($"Invalid Hscode");
            }
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
            {
                if (!match.Success)
                {
                    Log.Error($"|ProductCodeValidation| Invalid Hscode");
                    throw new System.Exception($"Invalid Hscode");
                }
            }

            // Product Code should be 4 digits(numeric value)

            if (productCode.Length != 4)
            {
                Log.Error($"|ProductCodeValidation| Invalid Product Code");
                throw new System.Exception($"Invalid Product Code");
            }

            // There should be no active same HsCode + Product Code combination having overlapping effective date and end date. 

            var overLappingProductCode = command.UnitOfWork.ProductCodeEntityRepository.GetOverlappingEffectiveFromProductCode(hSCode, productCode, effectiveFromDt);
            if (overLappingProductCode.Count > 0)
            {
                Log.Error($"|ProductCodeValidation| Product Code is overlapping with existing product code");
                throw new System.Exception($"Invalid Product Code is overlapping with existing product code");
            }


        }

    }
}