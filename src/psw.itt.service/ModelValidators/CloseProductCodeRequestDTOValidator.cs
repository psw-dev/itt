using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.SD.Service.ModelValidators
{
    public class CloseProductCodeRequestDTOValidator : AbstractValidator<CloseProductCodeRequestDTO>
    {
        public CloseProductCodeRequestDTOValidator()
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


        }
    }
}