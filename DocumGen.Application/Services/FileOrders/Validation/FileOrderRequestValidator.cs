using DocumGen.Application.Services.FileOrders.Models;
using FluentValidation;
using System;

namespace DocumGen.Application.Services.FileOrders.Validation
{
    public class FileOrderRequestValidator : AbstractValidator<FileOrderRequest>
    {
        public FileOrderRequestValidator()
        {
            RuleFor(r => r.FileOrderId)
                .NotEqual(r => Guid.Empty).WithMessage("{PropertyName} is required.");
        }
    }
}
