using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class GetListOfAgencyAgainstHsCodeRequestDTOValidator : AbstractValidator<GetListOfAgencyAgainstHsCodeRequest>
    {
        public GetListOfAgencyAgainstHsCodeRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.tradeTranTypeId)
            .NotEmpty()
            .NotNull()
            .WithName("tradeTranTypeId")
            .WithMessage("'{PropertyName}' should not be empty.");
            
            
            RuleFor(model => model.HsCode)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be null.")
            .Must(isValidHscode).WithMessage("HS Code is not in correct format.");

            RuleFor(model => model.DocumentCode)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be null.");
            
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