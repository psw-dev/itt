using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class GetFactorLOVItemsRequestDTOValidator : AbstractValidator<GetFactorLovItemsRequest>
    {
        public GetFactorLOVItemsRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

        
            RuleFor(c => c.AgencyId)
            .NotEmpty()
            .NotNull()
            .WithName("AgencyId")
            .WithMessage("'{PropertyName}' should not be empty.");

            RuleFor(c => c.TradeTranTypeID)
            .NotEmpty()
            .NotNull()
            .WithName("tradeTranTypeID")
            .WithMessage("'{PropertyName}' should not be empty.");
            
            RuleForEach(model => model.FactorList)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be empty.");
            
            RuleFor(model => model.HSCodeExt)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be empty.")
            .Must(isValidHscodeExt).WithMessage("HsCodeExt is not in correct format.");
            
        }
        protected bool isValidHscodeExt(string hsCodeExt)
        {
            bool isValidHscodeEsx = false;
            Regex reg = new Regex(@"([0-9]{4})\.([0-9]{4})\.([0-9]{4})");
            isValidHscodeEsx = reg.IsMatch(hsCodeExt);
            return isValidHscodeEsx;
        }
        
    }
}