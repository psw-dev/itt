using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.ModelValidators
{
    public class EditProductCodeRequestDTOValidator : AbstractValidator<EditProductCodeRequestDTO>
    {
        public EditProductCodeRequestDTOValidator()
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