using DocumGen.Application.Services.FileOrders.Models;
using FluentValidation;

namespace DocumGen.Application.Services.FileOrders.Validation
{
    public class FileOrderCreateRequestValidator : AbstractValidator<FileOrderCreateRequest>
    {
        public FileOrderCreateRequestValidator()
        {
            RuleFor(r => r.FileName)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
