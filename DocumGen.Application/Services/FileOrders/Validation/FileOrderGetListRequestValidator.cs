using DocumGen.Application.Services.FileOrders.Models;
using FluentValidation;

namespace DocumGen.Application.Services.FileOrders.Validation
{
    public class FileOrderGetListRequestValidator : AbstractValidator<FileOrderGetListRequest>
    {
        private const int MaxPageSize = 1000;
        private const int MinPageSize = 1;

        private const int MinPage = 1;

        public FileOrderGetListRequestValidator()
        {

            RuleFor(r => r.PageSize)
                .Must(x => x <= MaxPageSize).WithMessage("{PropertyName} must be no more then " + MaxPageSize)
                .Must(x => x >= MinPageSize).WithMessage("{PropertyName} must be no less then " + MinPageSize);

            RuleFor(r => r.PageNumber)
                .Must(x => x >= MinPage).WithMessage("{PropertyName} must be no less then " + MinPage);
        }
    }
}
