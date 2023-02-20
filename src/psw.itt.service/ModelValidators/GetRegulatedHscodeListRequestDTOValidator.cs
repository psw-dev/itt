using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class GetRegulatedHscodeListRequestDTOValidator : AbstractValidator<GetRegulatedHscodeListRequest>
    {
        public GetRegulatedHscodeListRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            // RuleFor(c => c.Chapter)
            // .NotEmpty()
            // .NotNull()
            // .WithName("Chapter")
            // .WithMessage("'{PropertyName}' should not be empty.");

            //  RuleFor(c => c.DocumentTypeCode)
            // .NotEmpty()
            // .NotNull()
            // .WithName("DocumentTypeCode")
            // .WithMessage("'{PropertyName}' should not be empty.");

            //  RuleFor(c => c.AgencyId)
            // .NotEmpty()
            // .WithName("AgencyId")
            // .WithMessage("'{PropertyName}' should not be empty.");
            
        }
    }
}