using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class GetRequirementMongoRequestDTOValidator : AbstractValidator<GetDocumentRequirementRequest>
    {
        public GetRequirementMongoRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(model => model.HsCode)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be null.")
            .Must(isValidHscode).WithMessage("HS Code is not in correct format.");

            RuleFor(c => c.AgencyId)
            .NotEmpty()
            .NotNull()
            .WithName("agencyId")
            .WithMessage("'{PropertyName}' should not be empty.");

            RuleFor(c => c.TradeTranTypeID)
            .NotEmpty()
            .NotNull()
            .WithName("tradeTranTypeID")
            .WithMessage("'{PropertyName}' should not be empty.");

            // RuleFor(c => c.Quantity)
            // .NotEmpty()
            // .NotNull()
            // .WithName("Quantity")
            // .WithMessage("'{PropertyName}' should not be empty.")
            // .PrecisionScale(9, 2, false)
            // .WithMessage("'{PropertyName}' should be two decimal places.");
            
            // RuleFor(c => c.AgencyQuantity)
            // .NotEmpty()
            // .NotNull()
            // .WithName("AgencyQuantity")
            // .WithMessage("'{PropertyName}' should not be empty.")
            // .PrecisionScale(9, 2, false)
            // .WithMessage("'{PropertyName}' should be two decimal places.");
            
            
            // RuleFor(model => model.IsFinancialRequirement)
            // .NotNull()
            // .NotEmpty()
            // .WithMessage("'{PropertyName}' should not be null.");
            
            // RuleFor(c => c.ImportExportValue)
            // .NotEmpty()
            // .NotNull()
            // .WithName("importExportValue")
            // .WithMessage("'{PropertyName}' should not be empty.")
            // .PrecisionScale(9, 2, false)
            // .WithMessage("'{PropertyName}' should be two decimal places.");
            
             RuleForEach(model => model.FactorCodeValuePair)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be empty.");
        }
        protected bool isValidHscode(string HSCode)
        {
            bool isValidHscode = false;
            Regex reg = new Regex(@"([0-9]{4})\.([0-9]{4})\.([0-9]{4})");
            isValidHscode = reg.IsMatch(HSCode);
            return isValidHscode;
        }
        
    }
}