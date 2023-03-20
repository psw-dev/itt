using System;
using System.Text.RegularExpressions;
using PSW.ITT.Service.Command;
using PSW.Lib.Logs;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PSW.ITT.Service.BusinessLogicLayer
{
    public class ProductCodeFileValidation
    {
        private string HsCode;
        private string columnValue;
        private string columnName;
        private short tradeTranTypeID;
        private int agencyID;
        private List<Data.DTO.ProductCodeValidationList> validation;
        private CommandRequest command; 
        
        public ProductCodeFileValidation(string hsCode, string columnValue, string columnName, List<Data.DTO.ProductCodeValidationList> validation, CommandRequest command, int agencyID, short tradeTranTypeID )
        {
            this.HsCode = hsCode;
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

                        string value = FactorList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).Item2;
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
                         if (!String.IsNullOrEmpty(columnValue)){
                            foreach(var i in  columnValue. Split(',') ){
                                var country = countryList.Find( x=>x.ToLower().Trim()==i.ToLower().Trim());
                                if(String.IsNullOrEmpty(country)){
                                    value.Add(i);
                                }
                               
                            }
                            if(value.Count>0)
                            {
                                Error = Error == "" ? columnName+" value "+(String.Join(",",value))+" does not exist in the system" : Error + ", " + columnName+" value "+(String.Join(",",value))+" does not exist in the system";
                            }
                            
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
                        var DocumentList = command.UnitOfWork.ProductCodeEntityRepository.GetDocumentLOV(agencyID, "DocumentType", "Name", tradeTranTypeID);
                        // var DocumentList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetDocumentLOV(item.TableName, item.ColumnName, item.Validation, agencyID);
                        var value = new List<string>();
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" is null" : Error + ", " + columnName+" is null";
                        }
                        else{
                            foreach(var i in  columnValue. Split(',') ){
                            var document = DocumentList.Find( x=>x.ItemValue.ToLower().Trim()==i.ToLower().Trim())?.ItemValue;
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
                        if(isProductCodeValid.Count < 1)
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
                        var bitList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetLOV(item.TableName, item.ColumnName);
                        if (!String.IsNullOrEmpty(columnValue)){
                           
                            string value = bitList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).Item2;
                            if (String.IsNullOrEmpty(value) )
                            {
                                Error = Error == "" ? columnName+" should be null or Yes/No" : Error + ", " + columnName+" should be null or Yes/No";
                            }
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
                            var DocumentList = command.UnitOfWork.ProductCodeEntityRepository.GetDocumentLOV(agencyID, "DocumentType", "Name", tradeTranTypeID);
                            // var DocumentList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetDocumentLOV(item.TableName, item.ColumnName, item.Validation, agencyID);
                            var value = new List<string>();
                            foreach(var i in  columnValue. Split(',') ){
                            var document = DocumentList.Find( x=>x.ItemValue.ToLower().Trim()==i.ToLower().Trim()).ItemValue;
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
                    case 21:
                    {  
                        
                        var HsCodeList = command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetListConsideringDate(item.TableName, item.ColumnName);
                        if (!String.IsNullOrEmpty(columnValue)){
                           
                            string value = HsCodeList.Find( x=>x.ToLower()==columnValue.ToLower());
                            if (String.IsNullOrEmpty(value) )
                            {
                                Error = Error == "" ? columnName+" does not exist in the system" : Error + ", " + columnName+" does not exist in the system";
                            }
                        }
                        
                        break;
                    }
                    case 22:
                    {  
                        
                        var activeProductCodeList = command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCode();
                        var productCodeList = command.UnitOfWork.ProductCodeEntityRepository.Get();
                        if (!String.IsNullOrEmpty(columnValue)){
                           
                            // var value = productCodeList.Any( x=>x.HSCode.ToLower()==HsCode && x.HSCodeExt==columnValue.ToLower());
                            if(!productCodeList.Any( x=>x.HSCode.ToLower()==HsCode && x.HSCodeExt==columnValue.ToLower()))// if (String.IsNullOrEmpty(value.ProductCode) )
                            {   
                                // availability = "No";
                                Error = Error == "" ? columnName+" does not exist in the system" : Error + ", " + columnName+" does not exist in the system";
                            }
                            
                        }
                        
                        break;
                    }
                    case 23:
                    case 24:
                    {  
                        
                        var calculationList = command.UnitOfWork.CommonForLovRepository.GetLOV(item.TableName, item.ColumnName);
                        if (String.IsNullOrEmpty(columnValue)){
                            Error = Error == "" ? columnName+" is null" : Error + ", " + columnName+" is null";
                        }
                        else{
                            string value = calculationList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).Item2;
                            if (String.IsNullOrEmpty(value) )
                            {
                                Error = Error == "" ? columnName+" is Required, please select one of the stated values, ["+String.Join(calculationList.Select(x=>x.Item2).ToString(),",")+"]" : Error + ", " + columnName+" is Required, please select one of the stated values, ["+String.Join(calculationList.Select(x=>x.Item2).ToString(),",")+"]";
                            }
                        }
                        break;
                    }
                    case 25:
                    {  
                        
                        var calculationList = command.UnitOfWork.CommonForLovRepository.GetLOV(item.TableName, item.ColumnName);
                        if (!String.IsNullOrEmpty(columnValue)){
                            string value = calculationList.Find( x=>x.Item2.ToLower()==columnValue.ToLower()).Item2;
                            if (String.IsNullOrEmpty(value) )
                            {
                                Error = Error == "" ? columnName+" is Required, please select one of the stated values, ["+String.Join(calculationList.Select(x=>x.Item2).ToString(),",")+"]" : Error + ", " + columnName+" is Required, please select one of the stated values, ["+String.Join(calculationList.Select(x=>x.Item2).ToString(),",")+"]";
                            }
                        }
                        break;
                    }
                    case 26:
                    {  
                        bool returnError=false;
                        var numericValidation = command.UnitOfWork.ValidationRepository.Where(new{ID=15}).FirstOrDefault();
                        var decimalValidation = command.UnitOfWork.ValidationRepository.Where(new{ID=7}).FirstOrDefault();
                        if(columnName.Contains("[Quantity;Unit;Price|]")){
                            string[] record = getValue(columnValue).Split('|');
                            foreach( var i in record){
                                string[] seperator = i.Split(';');

                                //Quantity Validation
                                if(seperator[0].Contains("-")){
                                    Match matchFrom = Regex.Match(seperator[0].Split('-').First().Trim(), numericValidation.Value, RegexOptions.IgnoreCase);
                                    Match matchTo = Regex.Match(seperator[0].Split('-').Last().Trim(), numericValidation.Value, RegexOptions.IgnoreCase);
                                    if (!matchFrom.Success || !matchTo.Success )
                                    {
                                        returnError = true;
                                        // Error = Error == "" ? columnName+" value is not in correct format as prescribed" : Error + ", " + columnName+" decimal is not in correct format";
                                    }
                                }
                                else{
                                    Match matchWhole = Regex.Match(seperator[0].Split('-').First().Trim(), numericValidation.Value, RegexOptions.IgnoreCase);
                                    if (!matchWhole.Success) returnError = true;
                                }

                                //Unit Validation
                                var unit = command.SHRDUnitOfWork.Ref_UnitsRepository.Where(new{Unit_Description = getLowerValue(seperator[1])}).FirstOrDefault();
                                if (unit==null ) returnError = true;

                                //Price Validation
                                Match matchD = Regex.Match(seperator[2].Trim(), decimalValidation.Value, RegexOptions.IgnoreCase);
                                Match matchN = Regex.Match(seperator[2].Trim(), numericValidation.Value, RegexOptions.IgnoreCase);
                                if (!matchD.Success && !matchN.Success) returnError = true;
                            }
                            if (returnError)
                            {
                                Error = Error == "" ? columnName+" value is not in correct format as prescribed" : Error + ", " + columnName+"  value is not in correct format as prescribed";
                            }
                        }
                        break;
                    }
                }
            }  
           return Error;

        }
        
        private string getValue(JToken str){
            return str.Value<string>();
        }
        private string getLowerValue(JToken str){
            return str.Value<string>().ToLower();
        }

    }
}