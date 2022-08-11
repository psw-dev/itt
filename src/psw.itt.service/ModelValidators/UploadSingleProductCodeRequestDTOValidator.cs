using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.SD.Service.ModelValidators
{
    public class UploadSingleProductCodeRequestDTOValidator : AbstractValidator<UploadSingleProductCodeRequestDTO>
    {
        public UploadSingleProductCodeRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.HSCode)
            .NotEmpty()
            .WithName("hSCode")
            .WithMessage("'{PropertyName}' should not be empty.");

            RuleFor(c => c.ProductCode)
            .NotNull()
            .WithName("productCode")
            .WithMessage("'{PropertyName}' should not be null.");

            RuleFor(c => c.Description)
            .NotNull()
            .WithName("description")
            .WithMessage("'{PropertyName}' should not be null.");


            RuleFor(c => c.EffectiveFromDt)
            .NotNull()
            .WithName("effectiveFromDt")
            .WithMessage("'{PropertyName}' should not be null.");

            RuleFor(c => c.EffectiveThruDt)
            .NotNull()
            .WithName("effectiveThruDt")
            .WithMessage("'{PropertyName}' should not be null.");


        }
    }
}