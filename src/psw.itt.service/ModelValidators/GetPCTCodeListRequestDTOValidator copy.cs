using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class ValidateRegulatedHSCodesRequestDTOValidator : AbstractValidator<ValidateRegulatedHSCodesRequestDTO>
    {
        public ValidateRegulatedHSCodesRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.TradeTranTypeId)
            .NotEmpty()
            .NotNull()
            .WithName("TradeTranTypeId")
            .WithMessage("'{PropertyName}' should not be empty.");
            
            RuleFor(c => c.AgencyID)
            .NotEmpty()
            .NotNull()
            .WithName("AgencyID")
            .WithMessage("'{PropertyName}' should not be empty.");
            
            // RuleFor(model => model.HSCodes)
            // .NotNull()
            // .WithMessage("'{PropertyName}' should not be null.")
            // .Must(isValidHscode).WithMessage("HS Code is not in correct format.");
            
        }
        protected bool isValidHscode(string HSCode)
        {
            bool isValidHscode = false;
            Regex reg = new Regex(@"([0-9]{4})\.([0-9]{4})");
            isValidHscode = reg.IsMatch(HSCode);
            return isValidHscode;
        }
        
    }
}