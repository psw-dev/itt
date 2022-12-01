using System;
using System.Text.RegularExpressions;
using PSW.ITT.Service.Command;
using PSW.Lib.Logs;
using System.Collections.Generic;


namespace PSW.ITT.Service.BusinessLogicLayer
{
    public class ProductCodeFileValidation
    {
        private string columnValue;
        private string columnName;
        private short tradeTranTypeID;
        private int agencyID;
        private List<Data.DTO.ProductCodeValidationList> validation;
        private CommandRequest command; 
        
        public ProductCodeFileValidation(string columnValue, string columnName, List<Data.DTO.ProductCodeValidationList> validation, CommandRequest command, int agencyID, short tradeTranTypeID )
        {
            this.columnValue = columnValue;
            this.columnName = columnName;
            this.validation = validation;
            this.agencyID = agencyID;
            this.tradeTranTypeID = tradeTranTypeID;
            this.command = command;
        }

        public string validate()
        {
            string Error ="";
            foreach (var item in validation)
            {
                switch(item.ValidationID)
                {
                    case 1:
                    {
                         if (String.IsNullOrEmpty(columnValue))
                        {
                            Error = Error+ " "+ columnName+" should not be null";
                        }
                        break;
                    }
                    case 2:
                    {  
                        Match match = Regex.Match(columnValue, item.Validation, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            Error = Error=="" ? "HS Code length is invalid" : Error+ ", HS Code length is invalid";
                        }
                        break;
                    }
                    case 3:
                    {  
                        Match match = Regex.Match(columnValue, item.Validation, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            Error = Error=="" ? "Product Code length is invalid" : Error+ ", Product Code length is invalid";
                        }
                        break;
                    }
                    case 4:
                    {  
                        
                        var FactorList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetLOV(item.TableName, item.ColumnName);

                         string value = FactorList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).ToString();
                        if(String.IsNullOrEmpty(value))
                        {
                            Error = Error == "" ? columnName+" value "+value+" does not exist in the system" : Error + ", " + columnName+" value "+value+" does not exist in the system";
                        }
                        break;
                    }
                    case 5:
                    {  
                        var countryList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetList(item.TableName, item.ColumnName);
                        var value = new List<string>();
                        foreach(var i in  columnValue. Split(',') ){
                            var country = countryList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                            if(String.IsNullOrEmpty(country)){
                                value.Add(i);
                            };
                        }
                        if(value.Count>0)
                        {
                            Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                        }
                        break;
                    }
                    case 6:
                    {  
                        var bitList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetLOV(item.TableName, item.ColumnName);
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" is null" : Error + ", " + columnName+" is null";
                        }
                        else{
                            string value = bitList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).Item2;
                            if (String.IsNullOrEmpty(value) )
                            {
                                Error = Error == "" ? columnName+" is Required" : Error + ", " + columnName+" is Required";
                            }
                        }
                        break;
                    }
                    case 7:
                    {  
                        
                        Match match = Regex.Match(columnValue, item.Validation, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                             Error = Error == "" ? columnName+" decimal is not in correct format" : Error + ", " + columnName+" decimal is not in correct format";
                        }
                        break;
                    }
                    case 8:
                    {  
                        
                        Match match = Regex.Match(columnValue, item.Validation, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                             Error = Error == "" ? columnName+"  is not in correct format, it can be 3 digit numbers" : Error + ", " + columnName+" is not in correct format, it can be 3 digit numbers";
                        }
                        break;
                    }
                    case 17:
                    case 16:
                    case 9:
                    {  
                        var DocumentList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetDocumentLOV(item.TableName, item.ColumnName, item.Validation, agencyID);
                        var value = new List<string>();
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" is null" : Error + ", " + columnName+" is null";
                        }
                        else{
                            foreach(var i in  columnValue. Split(',') ){
                            var document = DocumentList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                                if(String.IsNullOrEmpty(document)){
                                    value.Add(i);
                                };
                            }
                            if(value.Count>0)
                            {
                                Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                            }
                        }
                        
                        break;
                    }
                    case 10:
                    {  
                        var unitList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetList(item.TableName, item.ColumnName);
                        var value = new List<string>();
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" is null" : Error + ", " + columnName+" is null";
                        }
                        else{
                            foreach(var i in  columnValue. Split(',') ){
                            var unit = unitList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                            if(String.IsNullOrEmpty(unit)){
                                value.Add(i);
                            };
                            }
                            if(value.Count>0)
                            {
                                Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                            }
                        }
                        break;
                    }
                    case 11:
                    {
                        var isProductCodeValid = command.UnitOfWork.ProductCodeEntityRepository.GetProductCodeValidity(columnValue, agencyID, tradeTranTypeID);

                        //  string value = FactorList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).ToString();
                        if(!isProductCodeValid)
                        {
                            Error = Error == "" ? columnName+" value "+columnValue+" does not exist in the system" : Error + ", " + columnName+" value "+columnValue+" does not exist in the system";
                        }
                        break;
                    }
                    case 12:
                    {
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" should not be null" : Error + ", " + columnName+" should not be null";
                        }
                        else if (Convert.ToInt32(columnValue) == 0)
                        {
                            Error = Error == "" ? columnName+" should not be 0" : Error + ", " + columnName+" should not be 0";
                        }
                        break;
                    }
                    case 13:
                    {
                         if (!String.IsNullOrEmpty(columnValue) && Convert.ToInt32(columnValue) != 1)
                        {
                            Error = Error == "" ? columnName+" should be null or 1 if it is Required" : Error + ", " + columnName+" should be null or 1 if it is Required";
                        }
                        break;
                    }
                    case 14:
                    {  
                        var portList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetList(item.TableName, item.ColumnName);
                        var value = new List<string>();
                        foreach(var i in  columnValue. Split(',') ){
                            var unit = portList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                            if(String.IsNullOrEmpty(unit)){
                                value.Add(i);
                            };
                        }
                        if(value.Count>0)
                        {
                            Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                        }
                        break;
                    }
                    case 15:
                    {  
                        
                        Match match = Regex.Match(columnValue, item.Validation, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                             Error = Error == "" ? columnName+"  is not in correct format, it can be 9 digit number" : Error + ", " + columnName+" is not in correct format, it can be 9 digit number";
                        }
                        break;
                    }
                    case 20:
                    case 19:
                    case 18:
                    {  
                        if(String.IsNullOrEmpty(columnValue)){
                            break;
                        }
                        else{
                            var DocumentList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetDocumentLOV(item.TableName, item.ColumnName, item.Validation, agencyID);
                            var value = new List<string>();
                            foreach(var i in  columnValue. Split(',') ){
                            var document = DocumentList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                                if(String.IsNullOrEmpty(document)){
                                    value.Add(i);
                                };
                            }
                            if(value.Count>0)
                            {
                                Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                            }
                            break;
                        }
                        
                    }
                }
            }  
           return Error;

           
            // // Product code end date can not be set as a previous date. It should always be today's or future date.
            // if (DateTime.Compare(DateTime.Now, effectiveThruDt) > 0)
            // {
            //     Log.Error($"|ProductCodeValidation| Product code end date can not be set as a previous date");
            //     throw new System.Exception($"the effective thru {effectiveThruDt} should always be current or future date");
            // }
            // // Product code thru date can not be set less than start date. It should always be greater than from date

            // if (DateTime.Compare(effectiveFromDt, effectiveThruDt) > 0)
            // {
            //     Log.Error($"|ProductCodeValidation| Product code end date can not be set before start date");
            //     throw new System.Exception($"the effective thru {effectiveThruDt} should always greater than Effective from date");
            // }

            // // HSCode lenght should be 8 digits (numeric value)
            // string pattern = @"([0-9]{4})\.([0-9]{4})";
            // string input = hSCode;
            // if (input.Length != 9)
            // {
            //     Log.Error($"|ProductCodeValidation| Invalid Hscode");
            //     throw new System.Exception($"Invalid Hscode {hSCode}");
            // }
            // Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            // if (!match.Success)
            // {
            //     Log.Error($"|ProductCodeValidation| Invalid Hscode");
            //     throw new System.Exception($"Invalid Hscode {hSCode}");
            // }

            // // Product Code should be 4 digits(numeric value)

            // if (productCode.Length != 4)
            // {
            //     Log.Error($"|ProductCodeValidation| Invalid Product Code");
            //     throw new System.Exception($"Invalid Product Code {productCode}");
            // }
            // match = Regex.Match(input, @"[0-9]{4}", RegexOptions.IgnoreCase);
            // if (!match.Success)
            // {
            //     Log.Error($"|ProductCodeValidation| Invalid Product Code");
            //     throw new System.Exception($"Invalid Product Code {productCode}");
            // }

            // // There should be no active same HsCode + Product Code combination having overlapping effective date and end date. 

            // var overLappingProductCode = command.UnitOfWork.ProductCodeEntityRepository.GetOverlappingProductCode(hSCode, productCode, effectiveFromDt, effectiveThruDt, tradeType);
            // if (overLappingProductCode.Count > 0)
            // {
            //     Log.Error($"|ProductCodeValidation| Product Code is overlapping with existing product code");
            //     throw new System.Exception($"Invalid! Product Code is overlapping with existing product code");
            // }


        }

    }
}